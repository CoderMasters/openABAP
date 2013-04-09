using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;

namespace openABAP.Compiler
{
	public class Program
	{
		private Dictionary<string, Class> ClassList = new Dictionary<string, Class>();
		public  string Name;
		public  List<System.Type> CreatedClasses = new List<System.Type>();

		public Program(string n)
		{
			this.Name = n;
		}
		
		public Class GetClass(string name) 
		{
			Class result = null;
			if ( ! ClassList.TryGetValue(name, out result) ) 
			{
				throw new CompilerError("unkown class " + name );
			}
			return result;
		}
		
		public void AddClass( Class aClass ) 
		{
			try
			{
				ClassList.Add( aClass.Name, aClass );
			} catch (ArgumentException) {
				throw new CompilerError("class " + aClass.Name + " already defined");
			}
		}

		public void BuildAssembly ()
		{
			string name        = this.Name;
			AssemblyName aName = new AssemblyName(name);
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly( 
			                       aName, 
			                       AssemblyBuilderAccess.RunAndSave);
			
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

			// create class inside module
			foreach (KeyValuePair<string, Class> pair in this.ClassList) {
				System.Type t = pair.Value.BuildAssembly( mb, doc );
				this.CreatedClasses.Add(t);
			}
			ab.Save(name + ".dll");
		}

		public void Run()
		{
			// execute
			System.Console.Write("Starting Program..."); 
			openABAP.Runtime.Runtime.init();
			foreach(System.Type t in this.CreatedClasses) {
				MethodInfo mi = t.GetMethod("Run");
				if (mi != null) {
					object o = Activator.CreateInstance(t);
					mi.Invoke(o, null);
					System.Console.WriteLine("ready");
					return;
				}
			}
			System.Console.WriteLine ("method 'Run' not found");
		}

	}
}

