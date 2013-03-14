using System;

namespace openABAP.Runtime
{
	
/// <summary>
/// value with abap type char, which represents a fixed length string with trailing spaces.
/// actually implemented with C# type string, which is not optimal.
/// NEW IMPLEMENTATION NEEDDED!
/// </summary>
	public class CharValue : IfValue
	{
   		public static string RuntimeClassname = "[Runtime]openABAP.Runtime.CharValue";
		
		private string Text = "";
		private int MaxLen   = 1;
		
		public CharValue (int len, string text=null)
		{
			this.MaxLen = len;
			if (text != null) {
				if (this.MaxLen >= text.Length) {
					this.Text = text;
				} else {
					this.Text = text.Substring(0,this.MaxLen);
				}				
			}
		}
		
		public InternalType GetInternalType()
		{
			return InternalType.c;
		}
		
		public string getRuntimeClassname()
		{
			return RuntimeClassname;
		}
		
		public int GetInt()
		{
			try	{
				return int.Parse(this.Text, System.Globalization.NumberStyles.Integer);
			} catch (FormatException) {
				throw new RuntimeError(this.Text + " is not an integer");
			} catch (OverflowException) {
				throw new RuntimeError("integer overflow");
			}
		}
		
		public double GetFloat()
		{
			try	{
				return double.Parse(this.Text, System.Globalization.NumberStyles.Integer);
			} catch (FormatException) {
				throw new RuntimeError(this.Text + " is not an integer");
			} catch (OverflowException) {
				throw new RuntimeError("integer overflow");
			}
		}
		
		public string GetString()
		{
			return this.Text;
		}
		
		public void Set( IfValue v )
		{
			this.Text = v.GetString();
		}
		
		public string OutputString()
		{
			return this.Text;
		}
		
		public IfValue Calculate(string op, IfValue v=null)
		{
			switch (op) {
				case "+": return new DecValue(this.GetFloat() + v.GetFloat());
				case "-": return new DecValue(this.GetFloat() - v.GetFloat());
				case "*": return new DecValue(this.GetFloat() * v.GetFloat());
				case "/": return new DecValue(this.GetFloat() / v.GetFloat());
				default:  return new DecValue(this.GetFloat());
			}
		}

	}
}

