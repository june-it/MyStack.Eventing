namespace MyStack.Eventing.RabbitMQ
{
    public static class EventMetaExtensions
    {
        public static EventMetadata AddRabbitMQHeaders(this EventMetadata meta, string name, string value)
        {
            meta.TryAdd($"{MyStackConsts.RABBITMQ_HEADER}{name}", value);
            return meta;
        }
    }
}
