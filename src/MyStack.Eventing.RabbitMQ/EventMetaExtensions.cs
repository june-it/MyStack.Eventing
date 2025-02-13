namespace MyStack.Eventing.RabbitMQ
{
    public static class EventMetaExtensions
    {
        public static EventMeta AddKeyValue(this EventMeta meta, string name, string value)
        {
            meta.TryAdd($"{RabbitMQConsts.RABBITMQ_HEADER}{name}", value);
            return meta;
        }
    }
}
