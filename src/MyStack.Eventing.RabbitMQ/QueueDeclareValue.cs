namespace MyStack.Eventing.RabbitMQ
{
    public class QueueDeclareValue
    {
        public string? Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
