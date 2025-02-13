namespace MyStack.Eventing.InMemory.Test
{
    public class FooEventHandler : IEventHandler<FooEvent>
    {
        public async Task HandleAsync(FooEvent @event, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }
    }
}
