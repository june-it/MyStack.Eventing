namespace MyStack.Eventing.RabbitMQ
{
    public class ExchangeDeclareValue
    {
        public string? ExchangeType { get; set; }
        public string? Name { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
