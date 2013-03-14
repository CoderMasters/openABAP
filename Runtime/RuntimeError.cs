using System;

namespace openABAP.Runtime
{
	public class RuntimeError: System.Exception
	{
	     public RuntimeError() : base () { }
	     public RuntimeError( string message) : base (message) { }
	     public RuntimeError( string message, System.Exception inner) : base (message, inner) { }
	}
}

