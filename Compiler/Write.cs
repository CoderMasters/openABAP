using System;
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
	}
}

