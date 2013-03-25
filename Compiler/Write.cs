using System;
using System.Reflection.Emit;
using openABAP;

namespace openABAP.Compiler
{
	public class Write : Command
	{
		private Value Value; 
		private string Format;
			
		public Write (Value val)
		{
			this.Value = val;
			this.Format = "";
		}

		public Write (Value val, string format)
		{
			this.Value = val;
			this.Format = format;
		}
		
		public void WriteCil( CilFile cil ) {
			if (Format == "/") {
				cil.WriteLine("call void class [mscorlib]System.Console::WriteLine()");			
			}
			this.Value.PushFormattedString( cil );  //push formatted string of output value to stack
			cil.WriteLine("call void class [mscorlib]System.Console::Write(string)");			
		}

		public void BuildAssembly( ILGenerator il )
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

