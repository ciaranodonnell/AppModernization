using Common.Messaging.Interfaces;
using System;

namespace Common.Messaging
{
	public interface IMessageBroker
	{

		void SendAMessage<TMessage>(string topic, TMessage message);

		IMessageSender<TMessage> GetMessageSender<TMessage>(string topic, IMessageSerializer serializer);
		
		IMessageReceiver<TMessage> SubscribeToMessages<TMessage>(string topic);



	}
}
