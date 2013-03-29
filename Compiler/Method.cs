using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;

namespace openABAP.Compiler
{
	public class Method
	{
		public string Name;
		public Visibility Visibilty; 
		public Boolean StaticMember;

		private DataList LocalData = new DataList();
		private CommandList Commands = new CommandList();
				
		public Method (string name, Visibility v = Visibility.publ, Boolean staticMember = false )
		{
			this.Name = name;
			this.Visibilty = v;
			this.StaticMember = staticMember;
		}
		
		public void AddLocalData( Data data)
		{
			this.LocalData.Add( data.Name, data );
		}
		
		public Data GetLocalData( string name )
		{
			Data result=null;
			this.LocalData.TryGetValue(name, out result);
			return result;
		}
		
		public void AddCommand( Command cmd )
		{
			this.Commands.Add(cmd);	
		}

		public void BuildAssembly(TypeBuilder tb, ISymbolDocumentWriter doc)
		{
			MethodAttributes attr = 0;
			switch (this.Visibilty)
			{
				case Visibility.publ: attr = MethodAttributes.Public;  break;
				case Visibility.prot: attr = MethodAttributes.Family;  break;
				case Visibility.priv: attr = MethodAttributes.Private; break;
			}
			if (this.StaticMember) {
				attr = attr  | MethodAttributes.Static;
			}

			MethodBuilder mb = tb.DefineMethod(this.Name, attr);
			ILGenerator il = mb.GetILGenerator();
			foreach( Command cmd in this.Commands ) 
			{
				il.MarkSequencePoint(doc, cmd.StartLine, cmd.StartCol, cmd.EndLine, cmd.EndCol);
				cmd.BuildAssembly(il);
			}
			il.Emit (OpCodes.Ret);
		}

		public void BuildAssembly (ILGenerator il)
		{
		}
	}
	
	public class MethodList : SortedList<string, Method> 
	{
	}
}

