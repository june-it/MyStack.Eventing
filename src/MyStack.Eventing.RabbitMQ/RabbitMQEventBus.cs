using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MyStack.Eventing.RabbitMQ
{
    public class RabbitMQEventBus(RabbitMQProvider rabbitMQProvider,
        QueueBindValueProvider queueBindValueProvider) : IEventBus
    {
        private readonly RabbitMQProvider _rabbitMQProvider = rabbitMQProvider;
        private readonly QueueBindValueProvider _queueBindValueProvider = queueBindValueProvider;
        private void SetHeaders(IEvent @event, IBasicProperties basicProperties)
        {
            var headersAttributes = @event.GetType().GetCustomAttributes<HeadersAttribute>();
            if (headersAttributes != null)
            {
                basicProperties.Headers ??= new Dictionary<string, object?>();
                foreach (var headersAttribute in headersAttributes)
                {
                    basicProperties.Headers.TryAdd(headersAttribute.Key, headersAttribute.Value);
                }
                var metaAttributes = @event.Meta.Where(x => x.Key.StartsWith(RabbitMQConsts.RABBITMQ_HEADER));
                if (metaAttributes.Any())
                {
                    foreach (var metaAttribute in metaAttributes)
                    {
                        var headerKey = metaAttribute.Key.Replace(RabbitMQConsts.RABBITMQ_HEADER, "");
                        basicProperties.Headers.TryAdd(headerKey, metaAttribute.Value);
                    }
                }
            }
        }
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            using var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            using (var channel = connection.CreateModel())
            {
                var basicProperties = channel.CreateBasicProperties();
                SetHeaders(@event, basicProperties);

                var sendData = JsonConvert.SerializeObject(@event);
                var sendBytes = Encoding.UTF8.GetBytes(sendData);

                var queueBindValue = _queueBindValueProvider.GetValue(@event.GetType());
                channel.BasicPublish(queueBindValue.Exchange, queueBindValue.RoutingKey!, true, basicProperties, sendBytes);
            }
        }
    }
}
