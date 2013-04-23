using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using openABAP.Runtime;

namespace openABAP.Compiler
{
	public enum Visibility { publ, prot, priv};

/// <summary>
/// central class of openABAP implementing following feauture:
/// scanning and parsing the abap source file.
/// creating an internal representation of the program using <see cref="openABAP.Compiler.Command"/> class.
/// building the CIL file 
/// assembling the CIL file to EXE-assembly file.
/// calling EXE-file 
/// </summary>
	public class Compiler
	{
		
		private Coco.Scanner Scanner = null;
		private Coco.Parser  Parser  = null; 
		private string       TempFilename = "";
		private StringWriter Errors = new StringWriter();

		public System.IO.FileInfo SourceFileInfo = null; 

		public  string ProgramName = "";
		public Visibility CurrentVisibility = Visibility.publ;

		// context during compilation
		private AssemblyBuilder ab;
		private ModuleBuilder module;
		private ISymbolDocumentWriter doc;
		private MethodBuilder mb = null;
		private ClassBuilder cb = null;
		private ILGenerator il = null;

		private Dictionary<string, ClassBuilder> classList = new Dictionary<string, ClassBuilder>();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="openABAP.Compiler.Compiler"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		public Compiler( String filename)
		{
			//new MemoryStream( System.Text.Encoding.UTF8.GetBytes( source ) );
			this.TempFilename = filename;
			this.Scanner = new Coco.Scanner(filename);
			this.Parser  = new Coco.Parser(this.Scanner);
			this.Parser.errors.errorStream = this.Errors;
			this.Parser.Context = this;
		}
		
		/// <summary>
		/// parsers the ABAP source fie and emits a CIL DLL file.
		/// the temporary source file is copied into respository
		/// </summary>
		public void Compile ()
		{

			System.Console.Write ("Parsing source...");
			this.Parser.Parse ();
			Console.WriteLine ();
			if (this.Parser.errors.count > 0) {
				throw new CompilerError (this.Parser.errors.count.ToString () + " errors dectected");
			} else {
				this.Save();
			}
			System.Console.WriteLine ("ready");

			// rename temporary filename into abap filename
			string sourceFilename = this.ProgramName + ".abap";
			if (File.Exists (sourceFilename)) {
				File.Delete (sourceFilename);
			}
			File.Move(this.TempFilename, sourceFilename);
		}

		public string GetErrors()
		{
			return this.Errors.ToString();
		}

		/// <summary>
		/// set program name and creates AssemblyBuilder, ModuleBuilder and debug Document
		/// </summary>
		/// <param name="n">N.</param>
		public void DefineProgram(string n)
		{
			this.ProgramName = n.ToUpper();
			
			// create AssemblyBuilder for RunAndSave
			AssemblyName aName = new AssemblyName(this.ProgramName);
			ab = AppDomain.CurrentDomain.DefineDynamicAssembly( aName, 
			                                                   AssemblyBuilderAccess.RunAndSave );
			
			// mark generated code as debuggable
			System.Type daType = typeof(DebuggableAttribute);
			ConstructorInfo daCtor = daType.GetConstructor(new System.Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { 
				DebuggableAttribute.DebuggingModes.DisableOptimizations | 
				DebuggableAttribute.DebuggingModes.Default });
			ab.SetCustomAttribute(daBuilder);
			
			// For a single-module assembly, the module name is usually
			// the assembly name plus an extension.
			module = ab.DefineDynamicModule(this.ProgramName, this.ProgramName + ".dll", true);
			
			// Tell Emit about the source file that we want to associate this with. 
			FileInfo info = new FileInfo(this.ProgramName + ".abap");
			doc = module.DefineDocument(info.FullName, Guid.Empty, Guid.Empty, Guid.Empty);
			
		}
		
		/// <summary>
		/// initialises a new class definition by creating the System.Refelection.Emit.RypeBuilder
		/// </summary>
		/// <param name="name">Name.</param>
		public void DefineClass(string name)
		{
			string classname = name.ToUpper();
			cb = new ClassBuilder( module.DefineType( classname, TypeAttributes.Public ) );
			classList.Add (classname,  cb);;
		}
		
		/// <summary>
		/// loads the TypeBuilder corresponding to the given name.
		/// The TypeBuilder is loaded in this.tb to implement it in following calls.
		/// </summary>
		/// <param name="name">Name.</param>
		public void ImplementClass(string name)
		{
			string classname = name.ToUpper();
			if (! this.classList.TryGetValue(classname, out this.cb) )
			{
				throw new CompilerError("unkown class " + classname);
			}
		}
		
		/// <summary>
		/// creates a MethodBuilder and stores is in this.mbList
		/// </summary>
		/// <param name="t">T.</param>
		public void DefineMethod(Coco.Token t, bool isStatic)
		{
			MethodAttributes attr = 0;
			switch (this.CurrentVisibility)
			{
			case Visibility.publ: attr = MethodAttributes.Public;  break;
			case Visibility.prot: attr = MethodAttributes.Family;  break;
			case Visibility.priv: attr = MethodAttributes.Private; break;
			}
			if (isStatic) {
				attr = attr  | MethodAttributes.Static;
			}
			mb = cb.DefineMethod(t.val.ToUpper(), attr);
		}
		
		/// <summary>
		/// loads the TypeBuilder which was creted during class definition.
		/// </summary>
		/// <param name="t">T.</param>
		public void ImplementMethod(Coco.Token t)
		{
			mb = cb.GetMethodBuilder ( t.val.ToUpper() );
			if (mb == null) {
				throw new CompilerError("unkown method " + t.val );
			} else {
				il =  mb.GetILGenerator();
			}
		}
		
		public void EmitCommand( ExecutableCommand cmd )
		{
			cmd.BuildAssembly( il );
		}
		
		/// <summary>
		/// at end the end of a method implementation we mus emit a RET command
		/// </summary>
		/// <param name="t">T.</param>
		public void EndMethod( Coco.Token t)
		{
			il.Emit (OpCodes.Ret);
		}
		
		public void EndClass()
		{
			cb.EndClass();
		}

		public FieldBuilder DefineField(string name, TypeDescr type, bool isStatic )
		{
			FieldAttributes attr = FieldAttributes.Private;
			switch (this.CurrentVisibility)
			{
			case Visibility.publ: attr = FieldAttributes.Public;  break;
			case Visibility.prot: attr = FieldAttributes.Family;  break;
			case Visibility.priv: attr = FieldAttributes.Private; break;
			}
			if (isStatic) {
				attr = attr | FieldAttributes.Static;
			}
			return cb.DefineField(name.ToUpper(), type, attr);
		}

		public IfVariable GetVariable(string name)
		{
			IfVariable result = null;
			if (mb != null)
			{
			} 
			if (result == null && cb != null)
			{
				FieldInfo fi = cb.GetFieldInfo(name.ToUpper());
				if (fi != null) {
					result =  new Field(fi);
				}
			}
			return result;
		}
		
		public void Save ()
		{
			ab.Save(this.ProgramName + ".dll");
		}
		
		/// <summary>
		/// starts the generated program. 
		/// this means scanning all generated classes for a method named "Run" and invoke it.
		/// </summary>
		public void Run()
		{
			// execute
			System.Console.Write("Starting Program..."); 
			openABAP.Runtime.Runtime.init();
			foreach(KeyValuePair<string, ClassBuilder> kvp in this.classList) {
				MethodInfo mi = kvp.Value.CreatedClass.GetMethod("RUN");
				if (mi != null) {
					object o = Activator.CreateInstance(kvp.Value.CreatedClass);
					mi.Invoke(o, null);
					System.Console.WriteLine("ready");
					return;
				}
			}
			System.Console.WriteLine ("method 'Run' not found");
		}

//		public String Exceute( )
//		{
//			string result = "";
//			if (ExeFileInfo != null)
//			{
//				Console.WriteLine("------------------------------------");				
//				Console.WriteLine ("starting ABAP program {0}", ExeFileInfo.Name);
//				Console.WriteLine("------------------------------------");				
//				System.Diagnostics.Process proc = new System.Diagnostics.Process();
//				proc.EnableRaisingEvents=false; 
//				proc.StartInfo.FileName = ExeFileInfo.FullName;
//				//proc.StartInfo.Arguments = 
//				proc.StartInfo.UseShellExecute = false;
//				proc.StartInfo.RedirectStandardOutput = true;
//				proc.Start();
//				proc.WaitForExit();
//				result = proc.StandardOutput.ReadToEnd();
//				Console.WriteLine( result );					
//				Console.WriteLine("------------------------------------");				
//				Console.WriteLine("ready");				
//				Console.WriteLine("------------------------------------");	
//			}
//			return result;
//		}
		
	}
}

