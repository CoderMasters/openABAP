using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;
using System.Threading;
using openABAP;

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

		public System.IO.FileInfo SourceFileInfo = null; 


		public Compiler( String filename)
		{
			//new MemoryStream( System.Text.Encoding.UTF8.GetBytes( source ) );
			this.TempFilename = filename;
			this.Scanner = new Coco.Scanner(filename);
			this.Parser  = new Coco.Parser(this.Scanner);
		}
		

		public System.Type Compile ()
		{

			System.Console.WriteLine ("Parsing source");
			this.Parser.Parse ();
			Console.WriteLine ();
			if (this.Parser.errors.count > 0) {
				throw new CompilerError (this.Parser.errors.count.ToString () + " errors dectected");
			}
			Console.WriteLine ("------------------------------------"); 

			// rename temporary filename into abap filename
			string sourceFilename = this.Parser.Program.Name + ".abap";
			if (File.Exists (sourceFilename)) {
				File.Delete (sourceFilename);
			}
			File.Move(this.TempFilename, sourceFilename);

			return this.BuildAssembly();
		}

		private System.Type BuildAssembly ()
		{
			string name = this.Parser.Program.Name;
			AssemblyName aName = new AssemblyName(name);
	        AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly( aName, AssemblyBuilderAccess.RunAndSave);

			// mark generated code as debuggable
        	System.Type daType = typeof(DebuggableAttribute);
        	ConstructorInfo daCtor = daType.GetConstructor(new System.Type[] { typeof(DebuggableAttribute.DebuggingModes) });
        	CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { 
            	DebuggableAttribute.DebuggingModes.DisableOptimizations | 
            	DebuggableAttribute.DebuggingModes.Default });
        	ab.SetCustomAttribute(daBuilder);

	        // For a single-module assembly, the module name is usually
	        // the assembly name plus an extension.
	        ModuleBuilder mb = ab.DefineDynamicModule(name, name + ".dll", true);

			// Tell Emit about the source file that we want to associate this with. 
			FileInfo info = new FileInfo(name + ".abap");
        	ISymbolDocumentWriter doc = mb.DefineDocument(info.FullName, Guid.Empty, Guid.Empty, Guid.Empty);

			System.Type t = this.Parser.Program.BuildAssembly(mb, doc);

			ab.Save(name + ".dll");

			return t;

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

