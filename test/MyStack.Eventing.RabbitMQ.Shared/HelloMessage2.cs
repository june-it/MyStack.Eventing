namespace MyStack.Eventing.RabbitMQ.Shared
{
    [QueueDeclare("Demo")]
    [ExchangeDeclare("Demo")]
    [QueueBind("HelloMessage")]
    public class HelloMessage2 : EventBase
    {

    }
}
