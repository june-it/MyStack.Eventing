namespace MyStack.Eventing
{
    public interface IEventBus
    {
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default!);
    }
}
