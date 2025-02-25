namespace MyStack.Eventing
{
    /// <summary>
    /// Represents the event bus.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event record.
        /// </summary>
        /// <param name="eventData">The event object.</param>
        /// <param name="metadata">The event metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, EventMetadata? metadata = null, CancellationToken cancellationToken = default!);
    }
}
