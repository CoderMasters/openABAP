using System;
using System.Reflection.Emit;
using openABAP;

namespace openABAP.Compiler
{
	public class Write : ExecutableCommand
	{
		public IfValue Value   = null; 
		public string Format = null;

		public Write(openABAP.Coco.Token t) 
			: base(t) 
		{
		}

		public override void BuildAssembly( ILGenerator il )
		{
			System.Type[] types = { typeof(Runtime.IfValue) };
			System.Reflection.MethodInfo mi = typeof(openABAP.Runtime.Runtime).GetMethod("Write", types);
			if (Format != null && Format.Equals("/")) {
				StringLiteral nl = new StringLiteral( "\n" );
				nl.PushValue(il);
				il.EmitCall(OpCodes.Call, mi, null);
			}
			//push formatted string of output value to stack
			this.Value.PushValue(il);
			il.EmitCall(OpCodes.Call, mi, null);		
		}
	}
}

