using System;
using System.IO;
using System.Net;
using System.Threading;
using Bend.Util;

namespace openABAP.Http
{
    public class Server : Bend.Util.HttpServer {
        public Server(int port)
            : base(port) {
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
			//compile
            p.writeSuccess();
			openABAP.Compiler.Compiler compiler = new openABAP.Compiler.Compiler(inputData.BaseStream);
			compiler.Compile();
			string result = compiler.Exceute();
			p.outputStream.WriteLine(result);
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
			p.outputStream.WriteLine ("Current Time: " + DateTime.Now.ToString ());
			p.outputStream.WriteLine ("url : {0}", p.http_url);
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
