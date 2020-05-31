using System;

namespace Common.Messaging
{
	public interface IMessageBroker
	{

		void SendAMessage<TMessage>(string topic, TMessage message);

		ISubscription<TMessage> SubscribeToMessages<TMessage>(string topic);



	}
}
