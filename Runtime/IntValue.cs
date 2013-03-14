using System;

namespace openABAP.Runtime
{
	
/// <summary>
/// value with abap type char, which represents a fixed length string with trailing spaces.
/// actually implemented with C# type string, which is not optimal.
/// NEW IMPLEMENTATION NEEDDED!
/// </summary>
	public class IntValue : IfValue
	{
		public static string RuntimeClassname = "[Runtime]openABAP.Runtime.IntValue";
	
		private int Value = 0;
		
		public IntValue (int v=0)
		{
			Value = v;		
		}
		
		public InternalType GetInternalType()
		{
			return InternalType.i;
		}

		public static string getRuntimClassname()
		{
			return RuntimeClassname;
		}
		
		public int GetInt()
		{
			return this.Value;
		}
		
		public double GetFloat()
		{
			return (float)this.Value;
		}
		
		public string GetString()
		{
			return this.Value.ToString();
		}
		
		public void Set( IfValue v )
		{
			this.Value = v.GetInt();
		}
		
		public string OutputString()
		{
			return this.Value.ToString("G", System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public IfValue Calculate(string op, IfValue v=null)
		{
			switch (op) {
				case "+": if (v != null) return new IntValue(this.Value + v.GetInt());
				                    else return new IntValue(this.Value); 
				case "-": if (v != null) return new IntValue(this.Value - v.GetInt());
				                    else return new IntValue(-this.Value);
				case "*": if (v != null) return new IntValue(this.Value * v.GetInt());
									else throw new RuntimeError("right operand missing in expression");
				case "/": if (v != null) return new IntValue(this.Value / v.GetInt());
									else throw new RuntimeError("right operand missing in expression");
				default:  throw new RuntimeError("unknown operator " + op + " in expression");
			}
		}
	}
}

