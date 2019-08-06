using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDCOutboxSender.Options
{
    public class OutboxSenderOptions
    {

        [Option('d', "db", Required = true)]
        public string DatabaseConnectionString { get; set; }

        [Option('p', "pollInterval", Required = false, Default = 10, HelpText = "The amount of time between checks for changes")]
        public int PollIntervalSeconds { get; set; } = 10;



    }
}
