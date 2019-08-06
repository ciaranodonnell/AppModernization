using CDC.Loan;
using CDCOutboxSender.Options;
using CommandLine;
using System;

namespace CDCOutboxSender
{
    class Program
    {
        static void Main(string[] args)
        {

            CommandLine.Parser.Default.ParseArguments<OutboxSenderOptions>(args).WithParsed<OutboxSenderOptions>((options) =>
            {
                Console.WriteLine("Starting...");

                var loanDataChangeProcessor = new LoanDataChangeProcessor(options.DatabaseConnectionString, options.PollIntervalSeconds);

                loanDataChangeProcessor.Start();

                Console.WriteLine("Running!");

                Console.ReadLine();

            });



        }

    }
}
