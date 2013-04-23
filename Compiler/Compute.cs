using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Compute : ExecutableCommand
	{
		public IfExpression Expression = null;
		public IfVariable Target = null;

		public Compute (openABAP.Coco.Token t)
			: base(t)
		{
		}

		public override void BuildAssembly( ILGenerator il )
		{
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			System.Type t = this.Target.GetRuntimeType();
			MethodInfo mi = t.GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Expression.PushValue( il );
			il.EmitCall (OpCodes.Callvirt, mi, null);
		}

	}
}

