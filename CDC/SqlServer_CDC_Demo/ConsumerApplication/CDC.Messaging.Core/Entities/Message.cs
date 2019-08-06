namespace CDC.Messaging.Core.Entities
{
    public class Message<TContent>
    {
        public TContent Content { get; }

        public Message(TContent content)
        {
            this.Content = content;
        }
    }
}
