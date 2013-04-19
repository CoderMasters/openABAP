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
			System.Type[] types = { typeof(String) };
			if (Format != null && Format.Equals("/")) {
				il.Emit(OpCodes.Ldstr, "\n");
				il.EmitCall(OpCodes.Call, typeof(openABAP.Runtime.Runtime).GetMethod("Write", types), null);
			}
			//push formatted string of output value to stack
			this.Value.PushFormattedString(il);
			il.EmitCall(OpCodes.Call, typeof(openABAP.Runtime.Runtime).GetMethod("Write", types), null);		
		}
	}
}

