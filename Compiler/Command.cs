using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	
/// <summary>
/// Command.
/// abstract basis class of all abap-commands
/// </summary>
	abstract public class Command
	{
		public int StartLine = 0;
		public int StartCol  = 0;
		public int EndLine   = 0;
		public int EndCol    = 0;

		public Command( openABAP.Coco.Token t )
		{
			this.StartLine = t.line;
			this.StartCol  = t.col;
		}

		public void End( openABAP.Coco.Token t )
		{
			this.EndLine = t.line;
			this.EndCol  = t.col;
		}

	}

	public abstract class ExecutableCommand : Command
	{
		abstract public void BuildAssembly(ILGenerator il);

		public ExecutableCommand (Coco.Token t)
			: base(t)
		{
		}
	}
	
/// <summary>
/// container for commands used in methods to store all commands.
/// </summary>
	public class CommandList : List<ExecutableCommand> 
	{
	}
}

