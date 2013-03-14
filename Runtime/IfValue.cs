using System;

namespace openABAP.Runtime
{
/// <summary>
/// Interface which runtime values (i.e. variable and constants) must implement.
/// Conversion to other ABAP types are realised by transfer to a CIL basic type (i.e. int).
/// The target value must continue conversion from the CIL type to it's internale type.
/// </summary>
	public interface IfValue
	{
		InternalType GetInternalType();
		
		int GetInt();
		double GetFloat();
		string GetString();
		
		void Set( IfValue v );
		IfValue Calculate(string op, IfValue v=null);
		
		string OutputString();
	}

}