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
    	private FileStream   Stream  = null;
		
		public  string Filename;
		public System.IO.FileInfo SourceFileInfo = null; 
		public System.IO.FileInfo ExeFileInfo    = null; 		
		public System.IO.FileInfo CilFileInfo    = null; 

/// <summary>
/// initializes a new instance of the <see cref="openABAP.Compiler.Compiler"/> class.
/// </summary>
/// <param name='filename'>
/// fullpath to abap source file.
/// </param>
		public Compiler( string filename )
		{
			this.Filename = filename;
			this.SourceFileInfo = new System.IO.FileInfo( filename );
			this.CilFileInfo    = new System.IO.FileInfo( this.SourceFileInfo.Name.Replace( SourceFileInfo.Extension, ".cil") );
			this.ExeFileInfo    = new System.IO.FileInfo( this.SourceFileInfo.Name.Replace( SourceFileInfo.Extension, ".exe") );
			
			this.Stream  = this.SourceFileInfo.OpenRead();
			this.Scanner = new Coco.Scanner(Stream);
			this.Parser  = new Coco.Parser(this.Scanner);
			this.Parser.Program = new Program( );
		}
		

		public void Compile() 
		{
			System.Console.WriteLine ("Parsing file {0}", this.Filename);
			this.Parser.Parse();
			Console.WriteLine();
			if (this.Parser.errors.count > 0)
			{
				throw new CompilerError( this.Parser.errors.count.ToString() + " errors dectected");
			}
			Console.WriteLine("------------------------------------"); 
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
//				proc.StartInfo.FileName = "ilasm";
//				proc.StartInfo.Arguments = CilFileInfo.FullName;
				proc.StartInfo.FileName = "cmd";
				proc.StartInfo.Arguments = "/C ilasm.bat " + CilFileInfo.FullName;
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.Start();
				proc.WaitForExit();
				string data = proc.StandardOutput.ReadToEnd();
				Console.WriteLine( data );
			}
		}
		
		public void Exceute( )
		{
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
				string data = proc.StandardOutput.ReadToEnd();
				Console.WriteLine( data );					
				Console.WriteLine("------------------------------------");				
				Console.WriteLine("ready");				
				Console.WriteLine("------------------------------------");				
			}
		}
		
	}
}

