using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Move : Command
	{
		private Value    Source;
		private Data     Target;
		
		public Move (Value source=null, Data target = null) 
		{
			this.Source = source;
			this.Target = target;
		}
		
		public void setSource(Value source)
		{
			this.Source = source;
		}
		
		public void setTarget(Data target)
		{
			this.Target = target;
		}
		
		public void WriteCil( CilFile cil )
		{
			this.Target.PushValue( cil );
			this.Source.PushValue( cil );
			cil.WriteLine("callvirt instance void class [Runtime]openABAP.Runtime.IfValue::Set(class [Runtime]openABAP.Runtime.IfValue)");
		}

		public void BuildAssembly (ILGenerator il)
		{
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			MethodInfo mi = typeof(openABAP.Runtime.IfValue).GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Source.PushValue( il );
			il.EmitCall(OpCodes.Callvirt, mi, null);
		}

	}
}

