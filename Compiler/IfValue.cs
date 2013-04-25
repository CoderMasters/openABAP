using System;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public interface IfValue
	{
		void PushValue( ILGenerator il );
		System.Type GetRuntimeType();
	}
}

