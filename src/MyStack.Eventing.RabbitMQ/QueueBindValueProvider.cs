using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace MyStack.Eventing.RabbitMQ
{
    public class QueueBindValueProvider
    {
        private readonly ExchangeDeclareValueProvider _exchangeDeclareValueProvider;
        private readonly QueueDeclareValueProvider _queueDeclareValueProvider;
        private readonly RabbitMQOptions _options;
        public QueueBindValueProvider(ExchangeDeclareValueProvider exchangeDeclareValueProvider,
            QueueDeclareValueProvider queueDeclareValueProvider,
            IOptions<RabbitMQOptions> optionsAccessor)
        {
            _exchangeDeclareValueProvider = exchangeDeclareValueProvider;
            _queueDeclareValueProvider = queueDeclareValueProvider;
            _options = optionsAccessor.Value;
        }
        public QueueBindValue GetValue([NotNull] Type eventType)
        {
            var exchangeDeclareValue = _exchangeDeclareValueProvider.GetValue(eventType, _options.ExchangeOptions);
            var queueDeclareValue = _queueDeclareValueProvider.GetValue(eventType, _options.QueueOptions);

            var queueBindAttribute = eventType.GetCustomAttribute<QueueBindAttribute>();

            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "NoEntryAssembly";

            string queueName, exchangeName, routingKey;
            if (queueBindAttribute != null)
            {
                queueName = (!string.IsNullOrEmpty(queueBindAttribute.QueueName) ? queueBindAttribute.QueueName : queueDeclareValue.Name ?? assemblyName)!;
                exchangeName = (!string.IsNullOrEmpty(queueBindAttribute.ExchangeName) ? queueBindAttribute.ExchangeName : exchangeDeclareValue.Name ?? assemblyName)!;
                routingKey = queueBindAttribute.RoutingKey;
            }
            else
            {
                queueName = queueDeclareValue.Name ?? assemblyName;
                exchangeName = exchangeDeclareValue.Name ?? assemblyName;
                routingKey = eventType.FullName!;
            }

            return new QueueBindValue(exchangeName, queueName, routingKey);
        }
    }
}
