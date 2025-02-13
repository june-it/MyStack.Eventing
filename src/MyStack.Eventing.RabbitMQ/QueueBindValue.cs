namespace MyStack.Eventing.RabbitMQ
{
    public class QueueBindValue
    {
        public QueueBindValue(string queue, string exchange, string routingKey)
        {
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
        }

        public string Queue { get; private set; } = default!;
        public string Exchange { get; private set; } = default!;
        public string RoutingKey { get; private set; } = default!;
    }
}
