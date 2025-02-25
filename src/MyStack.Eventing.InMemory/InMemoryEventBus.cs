using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyStack.Eventing.InMemory
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryEventBus> _logger;
        public InMemoryEventBus(IServiceProvider serviceProvider,
            ILogger<InMemoryEventBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync(object eventData, EventMetadata? metadata = null, CancellationToken cancellationToken = default)
        {
            var eventType = eventData.GetType();
            var eventHandlers = _serviceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(eventType));
            if (eventHandlers.Any())
            {
                var tasks = eventHandlers.Select(async eventHandler =>
                {
                    try
                    {
                        await ((dynamic)eventHandler!).HandleAsync((dynamic)eventData, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, $"An exception occurred in the event handler '{eventHandler!.GetType().FullName}': {ex.Message}");
                    }
                });
                await Task.WhenAll(tasks);
            }
        }
    }
}
