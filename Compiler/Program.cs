using System;
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

		public System.Type BuildAssembly (ModuleBuilder mb, ISymbolDocumentWriter doc)
		{

			System.Type t = null;

			foreach (KeyValuePair<string, Class> pair in this.ClassList) {
				t = pair.Value.BuildAssembly( mb, doc );
			}

			return t;
		}

	}
}

