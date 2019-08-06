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
                Console.WriteLine("Starting...");





                Console.WriteLine("Running!");

                Console.ReadLine();



            });



        }

    }
}
