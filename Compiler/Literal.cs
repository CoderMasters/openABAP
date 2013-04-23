using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class StringLiteral : IfValue 
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

		public System.Type GetRuntimeType()
		{
			return typeof(openABAP.Runtime.CharValue);
		}

		public void PushFormattedString( ILGenerator il )
		{
			il.Emit (OpCodes.Ldstr, this.Text);
		}

		public void PushValue( ILGenerator il )
		{
			System.Type[] types = new System.Type[2];
			types[0] = typeof(int);
			types[1] = typeof(string);
			ConstructorInfo ci = typeof(openABAP.Runtime.CharValue).GetConstructor(types);
			il.Emit (OpCodes.Ldc_I4, this.Text.Length);
			il.Emit (OpCodes.Ldstr, this.Text);
			il.Emit (OpCodes.Newobj, ci);
		}
	}

	public class IntegerLiteral : IfValue 
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

		public System.Type GetRuntimeType()
		{
			return typeof(openABAP.Runtime.IntValue);
		}

		public void PushFormattedString( ILGenerator il )
		{
			il.Emit (OpCodes.Ldstr, this.ToString ());
		}
		public void PushValue( ILGenerator il )
		{
			System.Type[] types = new System.Type[1];
			types[0] = typeof(int);
			ConstructorInfo ci = typeof(openABAP.Runtime.IntValue).GetConstructor(types);
			il.Emit (OpCodes.Ldc_I4, this.Value);
			il.Emit (OpCodes.Newobj, ci);
		}
	}
	
}

