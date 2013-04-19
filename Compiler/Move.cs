using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class Move : ExecutableCommand
	{
		public IfValue    Source;
		public Data     Target;
		
		public Move (openABAP.Coco.Token t) 
			: base(t)
		{
		}
		
		public void setSource(IfValue source)
		{
			this.Source = source;
		}
		
		public void setTarget(Data target)
		{
			this.Target = target;
		}

		public override void BuildAssembly (ILGenerator il)
		{
			System.Type targetType = this.Target.getType().getRuntimeType();
			System.Type[] types = {typeof(openABAP.Runtime.IfValue)};
			MethodInfo mi = targetType.GetMethod("Set", types);
			this.Target.PushValue( il );
			this.Source.PushValue( il );
			il.EmitCall(OpCodes.Callvirt, mi, null);
		}

	}
}

