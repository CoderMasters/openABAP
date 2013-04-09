using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;
using System.Threading;
using openABAP;

namespace openABAP.Compiler
{
	public enum Visibility { publ, prot, priv};

/// <summary>
/// central class of openABAP implementing following feauture:
/// scanning and parsing the abap source file.
/// creating an internal representation of the program using <see cref="openABAP.Compiler.Command"/> class.
/// building the CIL file 
/// assembling the CIL file to EXE-assembly file.
/// calling EXE-file 
/// </summary>
	public class Compiler
	{
		
		private Coco.Scanner Scanner = null;
		private Coco.Parser  Parser  = null; 
		private string       TempFilename = "";
		private StringWriter Errors = new StringWriter();

		public System.IO.FileInfo SourceFileInfo = null; 


		public Compiler( String filename)
		{
			//new MemoryStream( System.Text.Encoding.UTF8.GetBytes( source ) );
			this.TempFilename = filename;
			this.Scanner = new Coco.Scanner(filename);
			this.Parser  = new Coco.Parser(this.Scanner);
			this.Parser.errors.errorStream = this.Errors;
		}
		

		public Program Compile ()
		{

			System.Console.Write ("Parsing source...");
			this.Parser.Parse ();
			Console.WriteLine ();
			if (this.Parser.errors.count > 0) {
				throw new CompilerError (this.Parser.errors.count.ToString () + " errors dectected");
			}
			System.Console.WriteLine ("ready");

			// rename temporary filename into abap filename
			string sourceFilename = this.Parser.Program.Name + ".abap";
			if (File.Exists (sourceFilename)) {
				File.Delete (sourceFilename);
			}
			File.Move(this.TempFilename, sourceFilename);

			System.Console.Write ("Creating Assembly...");
			Program result = this.BuildAssembly();
			System.Console.WriteLine ("ready");
			return result;
		}

		private Program BuildAssembly ()
		{
			this.Parser.Program.BuildAssembly();
			return this.Parser.Program;
		}

		public string GetErrors()
		{
			return this.Errors.ToString();
		}

//		public String Exceute( )
//		{
//			string result = "";
//			if (ExeFileInfo != null)
//			{
//				Console.WriteLine("------------------------------------");				
//				Console.WriteLine ("starting ABAP program {0}", ExeFileInfo.Name);
//				Console.WriteLine("------------------------------------");				
//				System.Diagnostics.Process proc = new System.Diagnostics.Process();
//				proc.EnableRaisingEvents=false; 
//				proc.StartInfo.FileName = ExeFileInfo.FullName;
//				//proc.StartInfo.Arguments = 
//				proc.StartInfo.UseShellExecute = false;
//				proc.StartInfo.RedirectStandardOutput = true;
//				proc.Start();
//				proc.WaitForExit();
//				result = proc.StandardOutput.ReadToEnd();
//				Console.WriteLine( result );					
//				Console.WriteLine("------------------------------------");				
//				Console.WriteLine("ready");				
//				Console.WriteLine("------------------------------------");	
//			}
//			return result;
//		}

	}
}

