using System;

namespace openABAP.Compiler
{
	public class CilFile
	{
		private string Filename;
		public System.IO.StreamWriter Stream;
		
		public string Namespace;
		public string Classname;
		
		public CilFile (string filename)
		{
			this.Filename = filename;
			System.Console.Write("writing CIL file {0} ...", this.Filename );
			this.Stream = new System.IO.StreamWriter(this.Filename);
			this.Stream.WriteLine(".assembly extern Runtime{}");
			this.Stream.WriteLine(".assembly extern mscorlib { .ver 4:0:0:0 } ");
		}
		
		public void Close()
		{
			this.Stream.Close();
			System.Console.WriteLine("ok");
		}
		
		public void WriteLine(string t)
		{
			Stream.WriteLine( t );
		}

		public void WriteLine(string t, object arg0)
		{
			Stream.WriteLine(t, arg0 );
		}

		public void WriteLine(string t, object arg0, object arg1)
		{
			Stream.WriteLine(t, arg0, arg1 );
		}

		public void WriteLine(string t, object arg0, object arg1, object arg2)
		{
			Stream.WriteLine(t, arg0, arg1, arg2 );
		}
	}
}

