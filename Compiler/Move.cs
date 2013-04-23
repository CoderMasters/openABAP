using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Move : ExecutableCommand
	{
		public IfValue    Source;
		public IfVariable Target;
		
		public Move (openABAP.Coco.Token t) 
			: base(t)
		{
		}

		public override void BuildAssembly (ILGenerator il)
		{
			System.Type targetType = this.Target.GetRuntimeType();
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			MethodInfo mi = targetType.GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Source.PushValue( il );
			il.EmitCall(OpCodes.Callvirt, mi, null);
		}

	}
}

