using System;
using System.Threading.Tasks;

namespace CDC.Messaging.Core.Interfaces
{
    public interface IMessagingService : IDisposable
    {
        void SendMessageToTopic<TMessage>(string topicName, TMessage messageContent, string correlationId = null);

        Task SendMessageToTopicAsync<TMessage>(string topicName, TMessage messageContent, string correlationId = null);

        ISubscriptionClient<TMessage> SubscribeToTopic<TMessage>(string topicName, string subscriptionQueueName = null);
    }
}