using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CustomerService.Data
{
	static class DbHelper
	{

		public static int GetInt32FromColumn(this IDataReader reader, string column)
		{
			return reader.GetInt32(reader.GetOrdinal(column));
		}
		public static string GetStringFromColumn(this IDataReader reader, string column)
		{
			return reader.GetString(reader.GetOrdinal(column));
		}
		public static DateTime GetDateTimeFromColumn(this IDataReader reader, string column)
		{
			return reader.GetDateTime(reader.GetOrdinal(column));
		}	
	}
}
