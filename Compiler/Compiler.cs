using System;
using System.IO;
using System.Diagnostics;
using openABAP;

namespace openABAP.Compiler
{
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
    	private Stream       Source  = null;
		
		public System.IO.FileInfo SourceFileInfo = null; 
		public System.IO.FileInfo ExeFileInfo    = null; 		
		public System.IO.FileInfo CilFileInfo    = null; 

		public Compiler( Stream source )
		{
			this.Source = source;
			//new MemoryStream( System.Text.Encoding.UTF8.GetBytes( source ) );

			this.Scanner = new Coco.Scanner(this.Source);
			this.Parser  = new Coco.Parser(this.Scanner);
		}
		

		public void Compile() 
		{
			System.Console.WriteLine ("Parsing source");
			this.Parser.Parse();
			Console.WriteLine();
			if (this.Parser.errors.count > 0)
			{
				throw new CompilerError( this.Parser.errors.count.ToString() + " errors dectected");
			}
			Console.WriteLine("------------------------------------"); 
			this.CilFileInfo    = new System.IO.FileInfo( this.Parser.Program.Name + ".cil" );
			this.ExeFileInfo    = new System.IO.FileInfo( this.Parser.Program.Name + ".exe" );

			CilFile cil = new CilFile( this.CilFileInfo.FullName );
			this.Parser.Program.WriteCil( cil );
			cil.Close();
			Console.WriteLine("------------------------------------");
			this.Ilasm( );
		}		

		public void Ilasm ( )
		{
			if (CilFileInfo != null) {
				System.Console.WriteLine( "ilasm - compiling CIL file...");
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.EnableRaisingEvents=false; 
				proc.StartInfo.FileName = "ilasm";
				proc.StartInfo.Arguments = CilFileInfo.FullName;
//				proc.StartInfo.FileName = "cmd";
//s				proc.StartInfo.Arguments = "/C ilasm.bat " + CilFileInfo.FullName;
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.Start();
				proc.WaitForExit();
				string data = proc.StandardOutput.ReadToEnd();
				Console.WriteLine( data );
			}
		}
		
		public String Exceute( )
		{
			string result = "";
			if (ExeFileInfo != null)
			{
				Console.WriteLine("------------------------------------");				
				Console.WriteLine ("starting ABAP program {0}", ExeFileInfo.Name);
				Console.WriteLine("------------------------------------");				
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.EnableRaisingEvents=false; 
				proc.StartInfo.FileName = ExeFileInfo.FullName;
				//proc.StartInfo.Arguments = 
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.Start();
				proc.WaitForExit();
				result = proc.StandardOutput.ReadToEnd();
				Console.WriteLine( result );					
				Console.WriteLine("------------------------------------");				
				Console.WriteLine("ready");				
				Console.WriteLine("------------------------------------");	
			}
			return result;
		}
		
	}
}

