using CDCOutboxSender.Options;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CDCOutboxSender
{
    class Program
    {
        static void Main(string[] args)
        {

            CommandLine.Parser.Default.ParseArguments<OutboxSenderOptions>(args).WithParsed<OutboxSenderOptions>((options) =>
            {

            CDCDataReader reader = new CDCDataReader("Customer_Audit", "dbo", connectionString, null);

            reader.Start(HandleRecords, HandleErrors);

            Console.WriteLine("Running...");

            Console.ReadLine();

            reader.Stop();


            });
            


        }

        private static void HandleErrors(Exception obj)
        {
            Console.WriteLine(obj.ToString());
            Process.GetCurrentProcess().Kill();
        }

        private static long? HandleRecords(IEnumerable<CDCRecord> arg)
        {

            long maxnum = 0;
            foreach(var record in arg)
            {
                //send on kafka?

                Console.WriteLine("Update Sending: " + Newtonsoft.Json.JsonConvert.SerializeObject(record));
                maxnum = record.UpdateId;
            }
            return maxnum;

        }

        private static string GetConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.ApplicationName = "CDCOutboxSender_Customer";
            connectionStringBuilder.DataSource = ".\\SQLEXPRESS";
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.InitialCatalog = "CDCTests";
            var connectionString = connectionStringBuilder.ToString();
            return connectionString;
        }
    }
}
