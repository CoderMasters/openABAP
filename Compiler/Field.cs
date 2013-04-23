using System;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	/// <summary>
	/// references to a field of a class.
	/// implements interface IfVariable and IfValue which makes it accessable by WRTE, MOVE, COMPUTE, ...
	/// </summary>
	public class Field : IfVariable 
	{	
		private FieldInfo fi;

		public Field( FieldInfo fi)
		{
			this.fi = fi;
		}
	
		public System.Type GetRuntimeType()
		{
			return this.fi.FieldType;
		}

		public void PushFormattedString( ILGenerator il )
		{
			this.PushValue( il );
			il.EmitCall (OpCodes.Callvirt, fi.FieldType.GetMethod("OutputString"), null);
		}

		public void PushValue( ILGenerator il )
		{
			if ( ( this.fi.Attributes & FieldAttributes.Static) == 0)
			{
				// instance field
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, this.fi);
			} else {
				// static field
				il.Emit(OpCodes.Ldsfld, this.fi);
			}
		}
	}
	
}

