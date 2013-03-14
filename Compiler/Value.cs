using System;

namespace openABAP.Compiler
{
	public interface Value
	{
		
		void PushFormattedString( CilFile cil );
		void PushValue( CilFile cil );
	}
	
}

