using System;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public interface Value
	{
		void PushFormattedString( ILGenerator il );
		void PushValue( ILGenerator il );
	}
	
}

