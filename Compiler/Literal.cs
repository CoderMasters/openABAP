using System;

namespace openABAP.Compiler
{
	public class StringLiteral : Value 
	{	
		private string Text = "";
		
		public StringLiteral( string val )
		{
			this.Text = val;
		}
		public override string ToString()
		{
			return this.Text;
		}
		
		public void PushFormattedString( CilFile cil )
		{
			cil.WriteLine("ldstr \"{0}\"", this.Text);
		}
		
		public void PushValue( CilFile cil )
		{
			cil.WriteLine("ldc.i4.s " + this.Text.Length);
			cil.WriteLine("ldstr \"{0}\"", this.Text);
			cil.WriteLine("newobj instance void class [Runtime]openABAP.Runtime.CharValue::'.ctor'(int32, string)");
		}

	}

	public class IntegerLiteral : Value 
	{
		private int Value = 0;
		
		public IntegerLiteral( int val )
		{
			this.Value = val;
		}
		public override string ToString()
		{
			return Convert.ToString(this.Value);
		}
		public void PushFormattedString( CilFile cil)
		{
			cil.WriteLine("ldstr \"{0}\"", this.ToString());
		}
		public void PushValue( CilFile cil )
		{
			cil.WriteLine("ldc.i4.s " + this.Value);
			cil.WriteLine("newobj instance void class [Runtime]openABAP.Runtime.IntValue::'.ctor'(int32)");
		}
	}
	
}

