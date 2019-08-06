using CDC.Messaging.Core.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Entities = CDC.Messaging.Core.Entities;

namespace CDC.Messaging.Kafka
{
    public class KafkaSubscriptionClient<TData> : ISubscriptionClient<TData>
    {
        private readonly string topicName;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public string Topic => topicName;

        public event EventHandler<MessagedReceivedArgumentEventArgs<TData>> MessageReceivedSuccessEvent;

        public event EventHandler<MessagedReceiveErrorArgumentEventArgs<TData>> MessageReceivedErrorEvent;

        public ILogger Logger { get; }

        public ISerializer Serializer { get; }

        public ConsumerConfig ConsumerConfig { get; }

        public KafkaSubscriptionClient(string topicName, ILogger logger, ISerializer serializer, ConsumerConfig consumerConfig)
        {
            this.topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            ConsumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

            StartSubscribption();
        }

        public void Unsubscribe()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                this.Logger.LogInformation($"Unsubscribing to topic {Topic}");
                cancellationTokenSource.Cancel();
            }
        }

        private void StartSubscribption()
        {
            var subscriptionThread = new Thread(new ParameterizedThreadStart((cancellationTokenSource) =>
            {

                int commitPeriod = 5;   // Should this be configurable with default value?

                CancellationTokenSource tokenSource = (CancellationTokenSource)cancellationTokenSource;

                using (var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
                {
                    consumer.Subscribe(Topic);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(tokenSource.Token);
                                if (consumeResult?.Message != null)
                                {
                                    if (!consumeResult.IsPartitionEOF)
                                    {
                                        NotifyMessageReceivedToSubscribers(consumeResult);

                                        this.Logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                                        if (consumeResult.Offset % commitPeriod == 0)
                                        {
                                            // The Commit method sends a "commit offsets" request to the Kafka
                                            // cluster and synchronously waits for the response. This is very
                                            // slow compared to the rate at which the consumer is capable of
                                            // consuming messages. A high performance application will typically
                                            // commit offsets relatively infrequently and be designed handle
                                            // duplicate messages in the event of failure.
                                            try
                                            {
                                                consumer.Commit(consumeResult);
                                            }
                                            catch (KafkaException e)
                                            {
                                                this.Logger.LogError($"Commit error: {e.Error.Reason}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Logger.LogInformation($"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                                    }
                                }
                            }
                            catch (ConsumeException e)
                            {
                                NotifyMessageErroredToSubscribers(e.ConsumerRecord.Value);
                                this.Logger.LogError($"Consume error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        this.Logger.LogError("Closing consumer.");
                    }
                }
            }));

            subscriptionThread.Start(cancellationTokenSource);

        }

        private void NotifyMessageReceivedToSubscribers(ConsumeResult<Ignore, string> consumeResult)
        {
            var message = new Entities.Message<TData>(Serializer.DeserializeFromString<TData>(consumeResult.Value));
            MessageReceivedSuccessEvent?.Invoke(this, new MessagedReceivedArgumentEventArgs<TData>() { ReceivedTimestamp = consumeResult.Timestamp.UtcDateTime, Message = message });
        }

        private void NotifyMessageErroredToSubscribers(byte[] result)
        {
            //TODO : Need to revisit this
            MessageReceivedErrorEvent?.Invoke(this, new MessagedReceiveErrorArgumentEventArgs<TData>() { ShouldSuicide = true, Message = result });
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}
