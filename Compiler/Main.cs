using System;

namespace openABAP.Compiler
{
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 1)
				Console.WriteLine("Syntax : openABAP <abap source file>");
			else {
				try {
					Compiler Compiler = new Compiler( args[0] );
					Compiler.Compile( );
					Compiler.Exceute( );
					
				} catch (System.Exception err) {
					Console.WriteLine("ERROR: " + err.Message);
				}
			}
		}			
	}
}
