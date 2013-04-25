using System;
using System.Data;
using System.Data.SQLite;

namespace Runtime.SQL
{
	public class Database
	{
		static private SQLiteConnection cnn = null;

		static Database()
		{
			try
			{
				if (cnn == null) {
					cnn = new SQLiteConnection("Data Source=openABAP.s3db;Version=3;New=True;");
					cnn.Open();
				}
			}
			catch (Exception fu)
			{
				// If an exception is thrown it is probably because the database already exists
				// ... just delete it, or add a delete clause at the beginning of your program
				throw new openABAP.Runtime.RuntimeError(fu.Message.ToString());
			} 
		}
	
	}
}

