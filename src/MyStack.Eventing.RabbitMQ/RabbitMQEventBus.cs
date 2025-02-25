using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MyStack.Eventing.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly RabbitMQProvider _rabbitMQProvider;
        private readonly QueueBindValueProvider _queueBindValueProvider;
        private readonly ILogger<RabbitMQEventBus> _logger;
        public RabbitMQEventBus(RabbitMQProvider rabbitMQProvider,
            QueueBindValueProvider queueBindValueProvider,
            ILogger<RabbitMQEventBus> logger)
        {
            _rabbitMQProvider = rabbitMQProvider;
            _queueBindValueProvider = queueBindValueProvider;
            _logger = logger;
        }

        private void SetHeaders(object eventData, IBasicProperties basicProperties)
        {
            var headersAttributes = eventData.GetType().GetCustomAttributes<HeadersAttribute>();
            if (headersAttributes != null)
            {
                basicProperties.Headers ??= new Dictionary<string, object?>();
                foreach (var headersAttribute in headersAttributes)
                {
                    basicProperties.Headers.TryAdd(headersAttribute.Key, headersAttribute.Value);
                }
            }
            if (eventData is IEvent @event)
            {
                var metaAttributes = @event.Metadata.Where(x => x.Key.StartsWith(MyStackConsts.RABBITMQ_HEADER));
                if (metaAttributes.Any())
                {
                    foreach (var metaAttribute in metaAttributes)
                    {
                        var headerKey = metaAttribute.Key.Replace(MyStackConsts.RABBITMQ_HEADER, "");
                        basicProperties.Headers.TryAdd(headerKey, metaAttribute.Value);
                    }
                }
            }
        }
        public async Task PublishAsync(object eventData, EventMetadata? metadata = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            using (var channel = connection.CreateModel())
            {
                var basicProperties = channel.CreateBasicProperties();
                object? messageData = eventData;

                if (eventData is IEvent @event)
                {
                    if (metadata != null)
                    {
                        foreach (var key in metadata.Keys)
                            @event.Metadata.TryAdd(key, metadata[key]);
                    }
                }
                else
                {
                    var eventWrapperType = typeof(EventWrapper<>).MakeGenericType(eventData.GetType());
                    messageData = Activator.CreateInstance(eventWrapperType, eventData);
                    if (metadata != null)
                    {
                        foreach (var key in metadata.Keys)
                            ((dynamic)messageData!).Metadata.TryAdd(key, metadata[key]);
                    }
                }

                SetHeaders(eventData, basicProperties);

                var sendData = JsonConvert.SerializeObject(messageData);
                var sendBytes = Encoding.UTF8.GetBytes(sendData);

                var queueBindValue = _queueBindValueProvider.GetValue(eventData.GetType());
                channel.BasicPublish(queueBindValue.ExchangeName, queueBindValue.RoutingKey!, true, basicProperties, sendBytes);
                _logger?.LogInformation($"[{queueBindValue.RoutingKey}] Published message: {sendData}.");
            }
        }
    }
}
