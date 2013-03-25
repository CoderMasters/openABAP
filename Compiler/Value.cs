using System;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public interface Value
	{
		
		void PushFormattedString( CilFile cil );
		void PushValue( CilFile cil );
		void PushFormattedString( ILGenerator il );
		void PushValue( ILGenerator il );
	}
	
}

