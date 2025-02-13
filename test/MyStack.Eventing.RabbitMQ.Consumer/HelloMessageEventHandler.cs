using MyStack.Eventing.RabbitMQ.Shared;

namespace MyStack.Eventing.RabbitMQ.Consumer
{
    public class HelloMessageEventHandler : IEventHandler<HelloMessage>
    {
        public async Task HandleAsync(HelloMessage @event, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Hello");
            await Task.CompletedTask;
        }
    }
}
