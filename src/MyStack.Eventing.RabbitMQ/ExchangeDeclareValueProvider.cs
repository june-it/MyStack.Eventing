using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MyStack.Eventing.RabbitMQ
{
    public class ExchangeDeclareValueProvider
    {
        public ExchangeDeclareValue GetValue([NotNull] Type eventType, [NotNull] ExchangeDeclareValue defaultValue)
        {
            var exchangeDeclareAttribute = eventType!.GetCustomAttribute<ExchangeDeclareAttribute>();
            if (exchangeDeclareAttribute == null)
                return defaultValue;
            ExchangeDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(exchangeDeclareAttribute.Name) ? exchangeDeclareAttribute.Name : defaultValue.Name,
                ExchangeType = !string.IsNullOrEmpty(exchangeDeclareAttribute.ExchangeType) ? exchangeDeclareAttribute.ExchangeType : defaultValue.ExchangeType,
                Durable = exchangeDeclareAttribute.Durable.HasValue ? exchangeDeclareAttribute.Durable.Value : defaultValue.Durable,
                AutoDelete = exchangeDeclareAttribute.AutoDelete.HasValue ? exchangeDeclareAttribute.AutoDelete.Value : defaultValue.AutoDelete,
                Arguments = exchangeDeclareAttribute.Arguments == null ? exchangeDeclareAttribute.Arguments : defaultValue.Arguments
            };
            return declareValue;
        }
    }
}
