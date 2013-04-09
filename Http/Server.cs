using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Bend.Util;

namespace openABAP.Http
{
    public class Server : Bend.Util.HttpServer {
        public Server(int port)
            : base(port) {
        }

		public static void Main (string[] args)
		{
			int port = 8080;
			Console.WriteLine("Starting http server on port {0}", port);
            Server httpServer = new Server(port);
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
			Console.WriteLine("listening on http://localhost:8080");
		}

        public override void handleGETRequest (HttpProcessor p)
		{
			Console.WriteLine ("request: {0}", p.http_url);
			if (p.http_url.Equals ("/")) {
				// return form 
				WriteForm (p);
			} else {
				//return static file
				try {
					FileInfo f = new FileInfo(p.http_url.TrimStart('/'));
					if (f.Exists) {
						StreamReader s = new StreamReader(f.FullName);
						p.writeSuccess();
						p.outputStream.Write(s.ReadToEnd());
					} else {
						p.writeFailure();
					}
				} catch(Exception e) {
					p.writeFailure();
				}
			}
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData) {
            Console.WriteLine("POST request: {0}", p.http_url);
			// write source in post body to server file
			StreamWriter file = new StreamWriter("temp.abap");
			inputData.BaseStream.CopyTo(file.BaseStream);
			file.Close();
			// compile
			openABAP.Compiler.Compiler compiler = new openABAP.Compiler.Compiler("temp.abap");
			try {
				openABAP.Compiler.Program prg = compiler.Compile();
				prg.Run ();
				p.outputStream.WriteLine(openABAP.Runtime.Runtime.Output);
			} catch (openABAP.Compiler.CompilerError ex) {
				p.outputStream.WriteLine(ex.Message);
				p.outputStream.WriteLine(compiler.GetErrors());
			}

			// ready
			//p.writeSuccess();
		}

		private void WriteForm (HttpProcessor p)
		{
			string source = "PROGRAM test.";
			try {
				StreamReader stream = new StreamReader ("Test.abap");
				source = stream.ReadToEnd ();
				stream.Close ();
			} catch (Exception e) {
			}

			p.writeSuccess ();
			p.outputStream.WriteLine ("<html>");
			p.outputStream.WriteLine ("<head>");
			p.outputStream.WriteLine ("<script src=\"client.js\" type=\"text/javascript\"></script>");
			p.outputStream.WriteLine ("</head>");
			p.outputStream.WriteLine ("<body>");
			p.outputStream.WriteLine ("<h1>openABAP</h1>");
			p.outputStream.WriteLine ("<form name=screen method=get action=\"\">");
			p.outputStream.WriteLine ("<textarea name=source cols=120 rows=30>");
			p.outputStream.WriteLine (source);
			p.outputStream.WriteLine ("</textarea>");
			p.outputStream.WriteLine ("<input type=submit name=sy-ucomm value=OK onclick=\"return send();\">");
			p.outputStream.WriteLine ("<br/><textarea name=result cols=120 rows=10 readonly></textarea>");
			p.outputStream.WriteLine ("</form></body></html>");
		}
    }

}
