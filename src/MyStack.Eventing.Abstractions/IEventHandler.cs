namespace MyStack.Eventing
{
    /// <summary>
    /// Represents the event handling service interface.
    /// </summary>
    /// <typeparam name="TEvent">Indicates the type of the event.</typeparam>
    public interface IEventHandler<TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// The task to handle the event.
        /// </summary>
        /// <param name="event">The event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default!);
    }
}
