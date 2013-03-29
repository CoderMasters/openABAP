using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public interface IfExpression
	{
		void PushValue( ILGenerator il );
	}
	
	public class BinaryExpression : IfExpression
	{
		string Operator;
		IfExpression LeftChild, RightChild;
		
		public BinaryExpression(IfExpression e1, string op, IfExpression e2)
		{
			LeftChild  = e1;
			Operator   = op;
			RightChild = e2;
		}

		public void PushValue (ILGenerator il)
		{
			System.Type[] types = { typeof(string), typeof(openABAP.Runtime.IfValue) };
			MethodInfo mi = typeof(openABAP.Runtime.IfValue).GetMethod("Calculate", types);
			this.LeftChild.PushValue(il); 
			il.Emit(OpCodes.Ldstr, this.Operator);
			this.RightChild.PushValue(il);
			il.EmitCall(OpCodes.Callvirt, mi, null);
		}

	}
	
	public class UnaryExpression : IfExpression
	{
		string Operator;
		IfExpression Expression;
		
		public UnaryExpression(string op, IfExpression e)
		{
			this.Operator = op;
			this.Expression = e;
		}

		public void PushValue( ILGenerator il )
		{
			System.Type[] types = { typeof(string), typeof(openABAP.Runtime.IfValue) };
			MethodInfo mi = typeof(openABAP.Runtime.IfValue).GetMethod("Calculate", types);
			il.Emit (OpCodes.Ldstr, this.Operator);
			il.Emit (OpCodes.Ldnull);
			il.EmitCall (OpCodes.Callvirt, mi, null);
		}
	}
	
	public class ExpressionLeaf : IfExpression
	{
		Value Value;
		
		public ExpressionLeaf( Value v )
		{
			this.Value = v;
		}

		public void PushValue( ILGenerator il )
		{
			Value.PushValue(il);
		}
	}
}

