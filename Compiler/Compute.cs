using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Compute : Command
	{
		public IfExpression Expression = null;
		public Data Target = null;
		
		public void WriteCil( CilFile cil )
		{
			this.Target.PushValue( cil );
			this.Expression.PushValue( cil );
			cil.WriteLine("callvirt instance void class [Runtime]openABAP.Runtime.IfValue::Set(class [Runtime]openABAP.Runtime.IfValue)");
		}

		public void BuildAssembly( ILGenerator il )
		{
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			MethodInfo mi = typeof(openABAP.Runtime.IfValue).GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Expression.PushValue( il );
			il.EmitCall (OpCodes.Callvirt, mi, null);
		}

	}
}

