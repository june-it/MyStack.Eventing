namespace MyStack.Eventing
{
    /// <summary>
    /// Represents a wrapper for events.
    /// </summary>
    /// <typeparam name="TEventData">Indicates the type of event data.</typeparam>
    public class EventWrapper<TEventData> : EventBase
    {
        public TEventData EventData { get; }
        public EventWrapper(TEventData eventData)
        {
            EventData = eventData;
        }
    }

    /// <summary>
    /// Represents the default wrapper for events.
    /// </summary>
    public class EventWrapper : EventWrapper<object>
    {
        public EventWrapper(object eventData) : base(eventData)
        {
        }
    }
}
