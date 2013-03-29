using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Compute : Command
	{
		public IfExpression Expression = null;
		public Data Target = null;

		public Compute (openABAP.Coco.Token t)
			: base(t)
		{
		}

		public override void BuildAssembly( ILGenerator il )
		{
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			MethodInfo mi = this.Target.getType().getRuntimeType().GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Expression.PushValue( il );
			il.EmitCall (OpCodes.Callvirt, mi, null);
		}

	}
}

