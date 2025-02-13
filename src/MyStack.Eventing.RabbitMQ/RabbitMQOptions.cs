namespace MyStack.Eventing.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string? RoutingKeyPrefix { get; set; }
        public ExchangeDeclareValue ExchangeOptions { get; set; } = new ExchangeDeclareValue();
        public QueueDeclareValue QueueOptions { get; set; } = new QueueDeclareValue();
    }

}
