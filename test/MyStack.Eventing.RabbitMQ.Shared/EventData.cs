namespace MyStack.Eventing.RabbitMQ.Shared
{
    public class EventData
    {
        public Guid Id { get; }
        public EventData()
        {
            Id = Guid.NewGuid();
        }
    }
}
