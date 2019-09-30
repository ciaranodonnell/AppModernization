﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace CDC.Common
{
    public class CDCHelper
    {

        public static SqlConnection GetConnection   (string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        private static string ConvertValueToStringOrNULL<T>(T value)
        {
            if (value is ValueType) return QuoteString(value.ToString());
            else if ((object)value == null)
            {
                return QuoteString(value.ToString());
            }
            else
            {
                return "NULL";
            }
        }
        private static string QuoteString(string value)
        {
            return $"N'{value}'";
        }
        private static string BoolAsInt(bool value)
        {
            return value ? "1" : "0";
        }



        public static void EnableCDCForTable(string connectionString, string schema, string table, string limitToRole = null, bool enableNetChanges = true)
        {
            using (var connection = GetConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sys.sp_cdc_enable_db";

                command.ExecuteNonQuery();

                command.CommandText = $"EXEC sys.sp_cdc_enable_table @source_schema = N'{schema}',  	@source_name = N'{table}',  	@role_name = {ConvertValueToStringOrNULL(limitToRole)}, @supports_net_changes = {BoolAsInt(enableNetChanges)}";

                command.ExecuteNonQuery();

            }
        }


    }
}
