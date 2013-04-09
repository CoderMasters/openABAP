using System;

namespace openABAP.Runtime
{
	public static class Runtime
	{
		public static string Output = "";

		public static void init()
		{
			Output = "";
		}

		static public void Write(string s)
		{
			Output = Output + s;
		}
	}
}

