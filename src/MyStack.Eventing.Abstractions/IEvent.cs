namespace MyStack.Eventing
{
    /// <summary>
    /// Represents the event interface
    /// </summary>
    public interface IEvent
    {
        EventMetadata Metadata { get; }
    }
}
