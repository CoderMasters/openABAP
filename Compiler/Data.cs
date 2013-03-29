using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Data : Value
	{
		public string Name;
		public Class  MemberOf;
		public Visibility Visibility; 
		public Boolean StaticMember;
		public FieldBuilder FieldBuilder = null;
		
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

		public void BuildAssembly (TypeBuilder tb)
		{
			FieldAttributes attr = FieldAttributes.Private;
			switch (this.Visibility)
			{
				case Visibility.publ: attr = FieldAttributes.Public;  break;
				case Visibility.prot: attr = FieldAttributes.Family;  break;
				case Visibility.priv: attr = FieldAttributes.Private; break;
			}
			if (this.StaticMember) {
				attr = attr | FieldAttributes.Static;
			}
	        this.FieldBuilder = tb.DefineField( this.Name, this.Type.getRuntimeType(), attr);
		}

		public void BuildAssembly (ILGenerator ctorIL)
		{
			System.Type rtc = this.Type.getRuntimeType ();
			System.Type[] types = null;
			if (! this.StaticMember) {
				ctorIL.Emit (OpCodes.Ldarg_0);
			}
			switch (this.Type.getInternalType ()) {
				case Runtime.InternalType.c:
					ctorIL.Emit (OpCodes.Ldc_I4, this.Type.getLength ()); //push 1st argument = text length
					ctorIL.Emit (OpCodes.Ldstr, "");                      //push 2nd argument = inital string
					types = new Type[2];
					types [0] = typeof(int);
					types [1] = typeof(string);
					ctorIL.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
					break;
				case Runtime.InternalType.n: 
					break;
				case Runtime.InternalType.d: 
					break;
				case Runtime.InternalType.t: 
					break;
				case Runtime.InternalType.i:
					ctorIL.Emit (OpCodes.Ldc_I4, 0);                   //push 1st argument = initial integer value
					types = new Type[1];
					types [0] = typeof(int);
					ctorIL.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
					break;
				case Runtime.InternalType.p: 
					ctorIL.Emit (OpCodes.Ldc_R8, 0.0);                 //push 1st argument = initial integer value
					types = new Type[1];
					types [0] = typeof(int);
					ctorIL.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
					break;
			}
			if (this.StaticMember) {
				ctorIL.Emit (OpCodes.Stsfld);
			} else {
				ctorIL.Emit (OpCodes.Stfld, this.FieldBuilder);
			}
		}

		public void PushFormattedString( ILGenerator il)
		{
			this.CallMethod(il, "OutputString");
		}

		public void PushValue( ILGenerator il )
		{
			if (this.MemberOf != null)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, this.FieldBuilder);
			} else {
				//MISSING: access of local data
			}
		}

		public void CallMethod( ILGenerator il, string method) 
		{
			MethodInfo mi = this.Type.getRuntimeType().GetMethod(method);
			this.PushValue( il );
			il.EmitCall (OpCodes.Callvirt, mi, null);
		}
	}
	
	public class DataList : SortedList<string, Data>
	{
	}
}

