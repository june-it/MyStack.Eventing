using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyStack.Eventing.RabbitMQ
{
    public class RabbitMQBackgroundService(
        IServiceProvider serviceProvider,
        RabbitMQProvider rabbitMQProvider,
        ExchangeDeclareValueProvider exchangeDeclareValueProvider,
        QueueDeclareValueProvider queueDeclareValueProvider,
        QueueBindValueProvider queueBindValueProvider,
        IEventingBuilder eventingBuilder,
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQBackgroundService> logger,
        SubscriptionManager subscriptionManager) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly RabbitMQProvider _rabbitMQProvider = rabbitMQProvider;
        private readonly ExchangeDeclareValueProvider _exchangeDeclareValueProvider = exchangeDeclareValueProvider;
        private readonly QueueDeclareValueProvider _queueDeclareValueProvider = queueDeclareValueProvider;
        private readonly QueueBindValueProvider _queueBindValueProvider = queueBindValueProvider;
        private readonly IEventingBuilder _eventingBuilder = eventingBuilder;
        private readonly RabbitMQOptions _options = options.Value;
        private readonly ILogger<RabbitMQBackgroundService> _logger = logger;
        private readonly SubscriptionManager _subscriptionManager = subscriptionManager;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            var channel = connection.CreateModel();
            var queueNames = BindRoutingKeysAndGetQueues(channel);
            await ReceiveMessageAsync(channel, queueNames, cancellationToken);
        }
        private List<string> BindRoutingKeysAndGetQueues(IModel channel)
        {
            List<string> queueNames = new();
            var eventTypes = _eventingBuilder.Assemblies.SelectMany(x =>
              x.GetTypes()
              .Where(x => !x.IsAbstract && x.GetInterfaces()
              .Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
              .SelectMany(x => x.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>)).SelectMany(x => x.GetGenericArguments())));
            if (!eventTypes.Any())
                return queueNames;
            foreach (var eventType in eventTypes)
            {
                var exchangeDeclareValue = _exchangeDeclareValueProvider.GetValue(eventType, _options.ExchangeOptions);
                var queueDeclareValue = _queueDeclareValueProvider.GetValue(eventType, _options.QueueOptions);
                var queueBindValue = _queueBindValueProvider.GetValue(eventType);

                channel.QueueDeclare(queueBindValue.Queue, queueDeclareValue.Durable, queueDeclareValue.Exclusive, queueDeclareValue.AutoDelete, queueDeclareValue.Arguments);
                channel.ExchangeDeclare(queueBindValue.Exchange, exchangeDeclareValue.ExchangeType ?? "topic", exchangeDeclareValue.Durable, exchangeDeclareValue.AutoDelete, exchangeDeclareValue.Arguments);
                channel.QueueBind(queueBindValue.Queue, queueBindValue.Exchange, queueBindValue.RoutingKey, null);
                queueNames.Add(queueBindValue.Queue);
                _subscriptionManager.Subscribe(eventType, queueBindValue.RoutingKey);
            }
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            return queueNames;
        }

        private async Task ReceiveMessageAsync(IModel channel, List<string> queueNames, CancellationToken cancellationToken)
        {
            if (queueNames.Count != 0)
            {
                foreach (var queueName in queueNames)
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (ch, ea) =>
                    {
                        var receivedMessage = Encoding.UTF8.GetString(ea.Body.Span);
                        _logger?.LogInformation($"Received message: {receivedMessage}.");
                        if (string.IsNullOrEmpty(receivedMessage))
                            return;
                        var subscriptions = _subscriptionManager.GetSubscriptions(ea.RoutingKey);
                        if (subscriptions == null)
                            return;
                        int successCount = 0;
                        var tasks = new List<Task>();
                        var semaphore = new SemaphoreSlim(10);
                        foreach (var subscription in subscriptions)
                        {
                            object? eventData = JsonConvert.DeserializeObject(receivedMessage, subscription);
                            if (eventData == null)
                                return;

                            var eventHandlers = _serviceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(subscription));
                            var subTasks = eventHandlers.Select(async eventHandler =>
                            {
                                await semaphore.WaitAsync(cancellationToken);
                                try
                                {
                                    await ((Task)((dynamic)eventHandler!).HandleAsync((dynamic)eventData, cancellationToken)).ConfigureAwait(false);
                                    Interlocked.Increment(ref successCount);
                                }
                                catch (Exception ex)
                                {
                                    _logger?.LogError(ex, $"An exception occurred in the event handler '{eventHandler!.GetType().FullName}': {ex.Message}");
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            });
                            tasks.AddRange(subTasks);
                        }
                        await Task.WhenAll(tasks);
                        if (successCount > 0)
                            channel.BasicAck(ea.DeliveryTag, false);
                        else
                            channel.BasicNack(ea.DeliveryTag, false, true);
                    };
                    channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
                    await Task.CompletedTask;
                }
            }
        }


    }
}
