using System;

namespace openABAP.Compiler
{
	public class Compute : Command
	{
		public IfExpression Expression = null;
		public Data Target = null;
		
		public void WriteCil( CilFile cil )
		{
			this.Target.PushValue( cil );
			this.Expression.PushValue( cil );
			cil.WriteLine("callvirt instance void class [Runtime]openABAP.Runtime.IfValue::Set(class [Runtime]openABAP.Runtime.IfValue)");
		}
	}
}

