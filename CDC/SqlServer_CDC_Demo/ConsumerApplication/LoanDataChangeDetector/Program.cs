using CDC.Loan;
using CDC.Messaging.Core.Interfaces;
using CDC.Messaging.Core.Serializers;
using CDCOutboxSender.Options;
using CommandLine;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CDCOutboxSender
{
    class Program
    {
        private static ILogger logger = null;
        private static ISerializer serializer = null;

        static void Main(string[] args)
        {

            // Configure Logger
            var serviceProvider = new ServiceCollection().AddLogging(loggingBuilder => loggingBuilder.AddConsole()).BuildServiceProvider();
            logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            // Configure Serializer
            serializer = new JSONSerializer();

            CommandLine.Parser.Default.ParseArguments<OutboxSenderOptions>(args).WithParsed<OutboxSenderOptions>((options) =>
            {
                Console.WriteLine("Starting...");

                var loanDataChangeProcessor = new LoanDataChangeProcessor(logger, serializer, GetProducerConfig(), GetSubscriberConfig(), options.DatabaseConnectionString, options.PollIntervalSeconds);

                loanDataChangeProcessor.Start();

                Console.WriteLine("Running!");

                Console.WriteLine("Press any key to Stop.");

                Console.ReadKey();

                loanDataChangeProcessor.Stop();

                Console.ReadLine();

            });



        }

        private static ProducerConfig GetProducerConfig()
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
            };
            return producerConfig;
        }

        private static ConsumerConfig GetSubscriberConfig()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                GroupId = "LoanGroup-1",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            return config;
        }

    }
}
