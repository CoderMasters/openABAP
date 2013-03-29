using System;
using System.Reflection.Emit;
using openABAP;

namespace openABAP.Compiler
{
	public class Write : Command
	{
		public Value Value; 
		public string Format;

		public Write(openABAP.Coco.Token t) 
			: base(t) 
		{
		}

		public override void BuildAssembly( ILGenerator il )
		{
			if (Format == "/") {
				il.EmitWriteLine("");
			}
			this.Value.PushFormattedString( il );  //push formatted string of output value to stack
			System.Type[] types = new System.Type[1];
			types[0] = typeof(String);
			il.EmitCall(OpCodes.Call, typeof(System.Console).GetMethod("Write", types), null);		
		}

	}
}

