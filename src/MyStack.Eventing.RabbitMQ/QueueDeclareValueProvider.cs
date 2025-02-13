using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MyStack.Eventing.RabbitMQ
{
    public class QueueDeclareValueProvider
    {
        public QueueDeclareValue GetValue([NotNull] Type eventType, [NotNull] QueueDeclareValue defaultValue)
        {
            var queueDeclareAttribute = eventType.GetCustomAttribute<QueueDeclareAttribute>();
            if (queueDeclareAttribute == null)
                return defaultValue;
            QueueDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(queueDeclareAttribute.Name) ? queueDeclareAttribute.Name : defaultValue.Name,
                Durable = queueDeclareAttribute.Durable.HasValue ? queueDeclareAttribute.Durable.Value : defaultValue.Durable,
                Exclusive = queueDeclareAttribute.Exclusive.HasValue ? queueDeclareAttribute.Exclusive.Value : defaultValue.Exclusive,
                AutoDelete = queueDeclareAttribute.AutoDelete.HasValue ? queueDeclareAttribute.AutoDelete.Value : defaultValue.AutoDelete,
                Arguments = queueDeclareAttribute.Arguments == null ? queueDeclareAttribute.Arguments : defaultValue.Arguments
            };
            return declareValue;
        }
    }
}
