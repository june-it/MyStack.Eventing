using MyStack.Eventing.RabbitMQ.Shared;

namespace MyStack.Eventing.RabbitMQ.Consumer
{
    public class HelloMessage2EventHandler : IEventHandler<HelloMessage2>
    {
        public async Task HandleAsync(HelloMessage2 @event, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Hello2");
            await Task.CompletedTask;
        }
    }
}
