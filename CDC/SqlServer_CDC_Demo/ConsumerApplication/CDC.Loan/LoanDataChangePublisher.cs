using CDC.Messaging.Core.Interfaces;
using CDC.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;

namespace CDC.Loan
{
    public class LoanDataChangePublisher : IDisposable
    {
        private readonly ISerializer serializer;
        private readonly ILogger logger;
        private readonly IMessagingService messagingService;

        public LoanDataChangePublisher(ILogger logger, ISerializer serializer, ProducerConfig producerConfig, ConsumerConfig consumerConfig)
        {
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            if (consumerConfig == null)
                throw new ArgumentNullException(nameof(consumerConfig));

            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            messagingService = new KafkaMessagingService(logger, serializer, producerConfig, consumerConfig);
        }

        public void Publish<TEventType>(string topicName, TEventType eventData)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            messagingService.SendMessageToTopic(topicName, eventData);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    messagingService.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LoanDataChangePublisher()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
