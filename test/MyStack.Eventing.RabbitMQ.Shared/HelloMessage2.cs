namespace MyStack.Eventing.RabbitMQ.Shared
{
    [QueueDeclare("Demo2")]
    [ExchangeDeclare("Demo")]
    [QueueBind("HelloMessage2")]
    public class HelloMessage2 : EventBase
    {

    }
}
