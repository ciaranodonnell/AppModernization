using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDCOutboxSender.Options
{
    public class OutboxSenderOptions
    {

        [Option]
        public string DatabaseConnectionString { get; set; }


        public int PollIntervalSeconds { get; set; } = 10;

    }
}
