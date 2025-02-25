namespace MyStack.Eventing
{
    /// <summary>
    /// Represents the base class for events 
    /// </summary>
    public abstract class EventBase : IEvent
    {
        public EventMetadata Metadata { get; }

        protected EventBase()
        {
            Metadata = new EventMetadata();
        }
    }
}
