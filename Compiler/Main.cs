using System;
using System.Threading;
using openABAP.Http;

namespace openABAP.Compiler
{
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			int port = 8080;
			Console.WriteLine("Starting http server on port {0}", port);
            Server httpServer = new Server(port);
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
			Console.WriteLine("listening on http://localhost:8080");
			//Compiler Compiler = new Compiler( args[0] );
			//Compiler.Compile( );
			//Compiler.Exceute( );
		}			
	}
}
