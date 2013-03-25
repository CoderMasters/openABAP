using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Program : Command
	{
		private Dictionary<string, Class> ClassList = new Dictionary<string, Class>();
		public  string Name;

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
		
		
		public void WriteCil( CilFile cil ) 
		{
			cil.Namespace = this.Name;
			
			cil.WriteLine(".assembly '" + this.Name + "' {}" );
			cil.WriteLine(".module " + this.Name + ".exe");
			cil.WriteLine(".namespace " + this.Name);
			cil.WriteLine("{" );
			
			foreach (KeyValuePair<string, Class> pair in this.ClassList) {
				pair.Value.WriteCil( cil );
			}
			
			cil.WriteLine("}");
		}

		public System.Type BuildAssembly ()
		{
			AssemblyName aName = new AssemblyName(this.Name);
	        AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly( aName, AssemblyBuilderAccess.RunAndSave);

	        // For a single-module assembly, the module name is usually
	        // the assembly name plus an extension.
	        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

			System.Type t = null;

			foreach (KeyValuePair<string, Class> pair in this.ClassList) {
				t = pair.Value.BuildAssembly( mb );
			}

			ab.Save(aName.Name + ".dll");
			return t;
		}

		public void BuildAssembly (ILGenerator il)
		{
		}
	}
}

