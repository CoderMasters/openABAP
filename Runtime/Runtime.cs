using System;

namespace openABAP.Runtime
{
	/// <summary>
	/// provide some basis routines during runtime of the ABAP program
	/// </summary>
	public static class Runtime
	{
		public static string Output = ""; //buffer for WRITE output

		/// <summary>
		/// initialize the WRITE buffer
		/// </summary>
		public static void init()
		{
			Output = "";
		}

		/// <summary>
		/// implements the WRITE function. it simply append the OutputString to the buffer.
		/// </summary>
		/// <param name="v">V.</param>
		static public void Write(IfValue v)
		{
			Output = Output + v.OutputString() + " ";
		}
	}
}

