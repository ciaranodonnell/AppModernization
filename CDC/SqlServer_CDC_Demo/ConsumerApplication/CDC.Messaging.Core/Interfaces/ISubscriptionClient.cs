
using CDC.Messaging.Core.Entities;
using System;

namespace CDC.Messaging.Core.Interfaces
{
    public interface ISubscriptionClient<TContent>
    {
        string Topic { get; }

        event EventHandler<MessagedReceivedArgumentEventArgs<TContent>> MessageReceivedSuccessEvent;

        event EventHandler<MessagedReceiveErrorArgumentEventArgs<TContent>> MessageReceivedErrorEvent;

        void Dispose();

        void Unsubscribe();
    }

    public class MessagedReceivedArgumentEventArgs<TContent> : EventArgs
    {
        public DateTimeOffset ReceivedTimestamp { get; set; }

        public Message<TContent> Message { get; set; }
    }

    public class MessagedReceiveErrorArgumentEventArgs<TContent> : EventArgs
    {
        public DateTimeOffset ReceivedTimestamp { get; set; }

        public byte[] Message { get; set; }

        ///<summary>
        /// Set this to true for this Subscription to throw an exception in the IMessagingClient to kill the whole process
        ///</summary>
        public bool ShouldSuicide { get; set; }

    }
}
