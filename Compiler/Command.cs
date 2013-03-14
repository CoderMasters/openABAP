using System;
using System.Collections.Generic;

namespace openABAP.Compiler
{
	
/// <summary>
/// Command.
/// abstract basis class of all abap-commands
/// </summary>
	public interface Command
	{
/// <summary>
/// writes all necessary commands to an cil file for implementing the runtime.
/// </summary>
/// <param name='cil'>
/// target cil file
/// </param>
		void WriteCil( CilFile cil );
	}
	
/// <summary>
/// container for commands used in methods to store all commands.
/// </summary>
	public class CommandList : List<Command> 
	{
	}
}
