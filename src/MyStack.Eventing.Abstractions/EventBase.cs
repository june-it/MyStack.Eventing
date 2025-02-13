namespace MyStack.Eventing
{
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; }
        public DateTime CreationTime { get; }

        public EventMeta Meta { get; }

        protected EventBase()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            Meta = [];
        }
    }
}
