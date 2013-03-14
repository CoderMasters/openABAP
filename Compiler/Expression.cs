using System;

namespace openABAP.Compiler
{
	public interface IfExpression
	{
		void PushValue( CilFile cil );
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
		
		public void PushValue( CilFile cil )
		{
			this.LeftChild.PushValue(cil); 
			cil.WriteLine("ldstr \"{0}\"", this.Operator);
			this.RightChild.PushValue(cil); 
			cil.WriteLine("callvirt instance class [Runtime]openABAP.Runtime.IfValue class [Runtime]openABAP.Runtime.IntValue::Calculate(string, class [Runtime]openABAP.Runtime.IfValue)");
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

		public void PushValue( CilFile cil )
		{
			this.Expression.PushValue(cil); 
			cil.WriteLine("ldstr \"{0}\"", this.Operator);
			cil.WriteLine("ldnull"); 
			cil.WriteLine("callvirt instance class [Runtime]openABAP.Runtime.IfValue class [Runtime]openABAP.Runtime.IntValue::Calculate(string, class [Runtime]openABAP.Runtime.IfValue)");
		}

	}
	
	public class ExpressionLeaf : IfExpression
	{
		Value Value;
		
		public ExpressionLeaf( Value v )
		{
			this.Value = v;
		}

		public void PushValue( CilFile cil )
		{
			Value.PushValue(cil);
		}
	}
}

