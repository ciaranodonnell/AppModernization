using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Interfaces
{
	public interface IMessageSender<TMessage>
	{
		void SendMessage(TMessage message);

	}
}
