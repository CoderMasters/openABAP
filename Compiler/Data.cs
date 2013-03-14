using System;
using System.Collections.Generic;

namespace openABAP.Compiler
{
	public class Data : Command, Value
	{
		public string Name;
		public Class  MemberOf;
		public Visibility Visibility; 
		public Boolean StaticMember;
		
		private Runtime.Type Type = null;
		
		public Data (string name = "" , Class c = null, Visibility v = Visibility.publ, Boolean staticMember = false )
		{
			this.Name = name;
			this.MemberOf = c;
			this.Visibility = v;
			this.StaticMember = staticMember;
		}
		
		public void setType(Runtime.Type aType)
		{
			this.Type = aType;
		}
		public Runtime.Type getType( )
		{
			return this.Type;
		}
		
		public void WriteCil( CilFile cil )
		{
			string cilType = "";
			string modifier = "public";
			string runtimeClass = this.Type.getRuntimeClassname();

			switch (this.Visibility)
			{
				case Visibility.publ: modifier = "public";    break;
				case Visibility.prot: modifier = "protected"; break;
				case Visibility.priv: modifier = "private";   break;
			}
			if (this.StaticMember) {
				modifier = modifier + " static";
			}
			
			cil.WriteLine(".field " + modifier + " class " + runtimeClass + " " + this.Name );
		}

		public void WriteCilNew( CilFile cil )
		{
			string store_field;
			string runtimeClass = this.Type.getRuntimeClassname();
		    if (this.StaticMember) {
				store_field = "stsfld";
			} else {
				cil.WriteLine("ldarg.0");                            	 //to create instanc attribute, we must push the this-instance
				store_field = "stfld";
			}
	
			switch (this.Type.getInternalType())
			{
				case Runtime.InternalType.c: 
					cil.WriteLine("ldc.i4.s " + this.Type.getLength() ); //push 1st argument = text length
					cil.WriteLine("ldstr \"\"");                         //push 2nd argument = inital string
				    cil.WriteLine("newobj instance void class " + runtimeClass + "::'.ctor'(int32, string)");
				    cil.WriteLine(store_field + " class " + runtimeClass + " {0}.{1}::{2}", cil.Namespace, cil.Classname, this.Name);
					break;
				case Runtime.InternalType.n: 
					break;
				case Runtime.InternalType.d: 
					break;
				case Runtime.InternalType.t: 
					break;
				case Runtime.InternalType.i:
					cil.WriteLine("ldc.i4.s " + "0" );                   //push 1st argument = initial integer value
				    cil.WriteLine("newobj instance void class " + runtimeClass + "::'.ctor'(int32)");
				    cil.WriteLine(store_field + " class " + runtimeClass + " {0}.{1}::{2}", cil.Namespace, cil.Classname, this.Name);
					break;
				case Runtime.InternalType.p: 
					cil.WriteLine("ldc.r8 " + "0." );                   //push 1st argument = initial float value
				    cil.WriteLine("newobj instance void class " + runtimeClass + "::'.ctor'(float64)");
				    cil.WriteLine(store_field + " class " + runtimeClass + " {0}.{1}::{2}", cil.Namespace, cil.Classname, this.Name);
					break;
			}
			
		}
		
		public void PushFormattedString( CilFile cil)
		{
			this.CallMethod(cil, "OutputString");
		}

		public void PushValue( CilFile cil )
		{
			string classname = this.Type.getRuntimeClassname();
			if (this.MemberOf != null)
			{
				string attribute = cil.Namespace + "." + this.MemberOf.Name + "::" + this.Name;
				cil.WriteLine("ldarg.0");
		        cil.WriteLine("ldfld class {0} {1}", classname, attribute);
			} else {
				//MISSING: access of local data
			}
		}
		
		public void CallMethod( CilFile cil, string method) 
		{
			string classname = this.Type.getRuntimeClassname();
			this.PushValue( cil );
			cil.WriteLine("callvirt instance string class {0}::{1}()", classname, method);
		}
				
	}
	
	public class DataList : SortedList<string, Data>
	{
	}
}

