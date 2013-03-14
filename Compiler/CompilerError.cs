using System;

namespace openABAP.Compiler
{
	public class CompilerError : System.Exception
	{
	     public CompilerError() : base () { }
	     public CompilerError( string message) : base (message) { }
	     public CompilerError( string message, System.Exception inner) : base (message, inner) { }
	}
}

