using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Interfaces
{
	public interface IMessageReceiver<TMessage>
	{

		event Action<object, MessageReceivedEventArgs<TMessage>> MessageReceived;

	}
}
