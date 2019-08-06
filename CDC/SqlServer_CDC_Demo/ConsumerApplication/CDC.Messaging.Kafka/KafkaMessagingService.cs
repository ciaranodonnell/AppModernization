using CDC.Messaging.Core.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CDC.Messaging.Kafka
{
    public class KafkaMessagingService : IMessagingService
    {

        public ILogger Logger { get; }

        public ISerializer Serializer { get; }

        public ProducerConfig ProducerConfig { get; }

        public ConsumerConfig ConsumerConfig { get; }

        public KafkaMessagingService(ILogger logger, ISerializer serializer, ProducerConfig producerConfig, ConsumerConfig consumerConfig)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
            ConsumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));
        }

        public void SendMessageToTopic<TMessage>(string topicName, TMessage messageContent, string correlationId = null)
        {
            SendMessageToTopicAsync(topicName, messageContent, correlationId).Wait();
        }

        public async Task SendMessageToTopicAsync<TMessage>(string topicName, TMessage messageContent, string correlationId = null)
        {
            if (topicName == null) throw new ArgumentNullException(nameof(topicName));

            using (var producer = new ProducerBuilder<Null, string>(ProducerConfig).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(topicName, new Message<Null, string> { Key = null, Value = Serializer.SerializeToString<TMessage>(messageContent) });

                    this.Logger.LogInformation($"Published to Topic:  {topicName} - DeliverResult: {result.Value}");
                }
                catch (ProduceException<string, string> e)
                {
                    this.Logger.LogError($"Error publishing message: {e.Message} {e.Error.Code}");
                    throw;
                }
            }

            await Task.FromResult(true);
        }

        public ISubscriptionClient<TMessage> SubscribeToTopic<TMessage>(string topicName, string subscriptionQueueName = null)
        {
            if (topicName == null) throw new ArgumentNullException(nameof(topicName));

            return new KafkaSubscriptionClient<TMessage>(topicName, Logger, Serializer, ConsumerConfig);
        }

        public void Dispose()
        {
            // Dispose Logic if any
        }
    }
}
