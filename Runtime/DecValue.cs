using System;

namespace openABAP.Runtime
{
/// <summary>
/// value with abap type char, which represents a fixed length string with trailing spaces.
/// actually implemented with C# type string, which is not optimal.
/// NEW IMPLEMENTATION NEEDDED!
/// </summary>
	public class DecValue : IfValue
	{
		public static string RuntimeClassname = "[Runtime]openABAP.Runtime.DecValue";
	
		private double Value = 0d;
		
		public DecValue (double v=0d)
		{
			Value = v;		
		}
		
		public InternalType GetInternalType()
		{
			return InternalType.p;
		}

		public string getRuntimeClassname()
		{
			return RuntimeClassname;
		}
		
		public int GetInt()
		{
			return (int)this.Value;
		}
		
		public double GetFloat()
		{
			return this.Value;
		}
		
		public string GetString()
		{
			return this.Value.ToString();
		}
		
		public void Set( IfValue v )
		{
			this.Value = v.GetFloat();
		}
		
		
		public IfValue Calculate(string op, IfValue v=null)
		{
			switch (op) {
				case "+": if (v != null) return new DecValue(this.Value + v.GetFloat());
				                    else return new DecValue(this.Value);
				case "-": if (v != null) return new DecValue(this.Value - v.GetFloat());
				                    else return new DecValue(-this.Value);
				case "*": if (v != null) return new DecValue(this.Value * v.GetFloat());
									else throw new RuntimeError("right operand missing in expression");
				case "/": if (v != null) return new DecValue(this.Value / v.GetFloat());
									else throw new RuntimeError("right operand missing in expression");
				default:  throw new RuntimeError("unknown operator " + op + " in expression");
			}
		}

		public string OutputString()
		{
			return this.Value.ToString("G", System.Globalization.CultureInfo.CurrentCulture);
		}

	}
}

