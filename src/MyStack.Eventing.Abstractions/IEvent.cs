namespace MyStack.Eventing
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime CreationTime { get; }
        EventMeta Meta { get; }
    }
}
