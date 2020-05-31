using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Interfaces
{
	public class MessageReceivedEventArgs<TMessage>
	{

		public TMessage Message { get; set; }

		public bool ShouldAcknowledgeMessage { get; set; }

	}
}
