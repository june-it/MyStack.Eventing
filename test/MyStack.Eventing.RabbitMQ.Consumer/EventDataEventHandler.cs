using MyStack.Eventing.RabbitMQ.Shared;

namespace MyStack.Eventing.RabbitMQ.Consumer
{
    public class EventDataEventHandler : IEventHandler<EventWrapper<EventData>>
    {
        public async Task HandleAsync(EventWrapper<EventData> @event, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("EventData");
            await Task.CompletedTask;
        }
    }
}
