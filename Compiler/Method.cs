using System;
using System.Collections.Generic;

namespace openABAP.Compiler
{
	public class Method : Command
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
		
		public void WriteCil( CilFile cil )
		{
			string modifier = "public";
			switch (this.Visibilty)
			{
				case Visibility.publ: modifier = "public";    break;
				case Visibility.prot: modifier = "protected"; break;
				case Visibility.priv: modifier = "private";   break;
			}
			if (this.StaticMember) {
				modifier = modifier + " static";
			}
			
			cil.WriteLine(".method " + modifier + " hidebysig instance default void " + this.Name + "()  cil managed ");
			cil.WriteLine("{");
			// write all commands to CIL 
			foreach( Command cmd in this.Commands ) 
			{
				cmd.WriteCil(cil);
			}
			//end of method
			cil.WriteLine("ret ");
			cil.WriteLine("} // end of method " + this.Name );
		}		
	}
	
	public class MethodList : SortedList<string, Method> 
	{
	}
}

