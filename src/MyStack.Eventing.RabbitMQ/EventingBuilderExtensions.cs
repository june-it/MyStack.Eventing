using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing.RabbitMQ
{
    public static class EventingBuilderExtensions
    {
        public static void UseRabbitMQ(this IEventingBuilder builder, Action<RabbitMQOptions> configureRabbitMQ)
        {
            builder.Services.AddTransient<RoutingKeyProvider>();
            builder.Services.AddTransient<QueueBindValueProvider>();
            builder.Services.AddTransient<ExchangeDeclareValueProvider>();
            builder.Services.AddTransient<QueueDeclareValueProvider>();
            builder.Services.AddTransient<RabbitMQProvider>();
            builder.Services.AddTransient<IEventBus, RabbitMQEventBus>();
            builder.Services.Configure(configureRabbitMQ);
            builder.Services.AddTransient<SubscriptionManager>();
            if (builder.Assemblies.Length != 0)
            {
                builder.Services.AddHostedService<RabbitMQBackgroundService>();
                var messageTypes = new List<Type>();
                var eventHandlerTypes = builder.Assemblies.SelectMany(x => x.GetTypes().Where(x => !x.IsAbstract && x.IsPublic && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
                if (eventHandlerTypes.Any())
                {
                    foreach (var eventHandlerType in eventHandlerTypes)
                    {
                        var handlerInterfaces = eventHandlerType.GetInterfaces()
                            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
                        foreach (var handlerInterface in handlerInterfaces)
                        {
                            builder.Services.AddTransient(typeof(IEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                            messageTypes.Add(handlerInterface.GetGenericArguments()[0]);
                        }
                    }
                }
                builder.Services.AddSingleton<ISubscriptionRegistrar, SubscriptionManager>();
                builder.Services.AddSingleton<SubscriptionManager>(factory =>
                {
                    var subscriptionManager = factory.GetRequiredService<ISubscriptionRegistrar>();
                    subscriptionManager.Subscribe(messageTypes.ToArray());
                    return (SubscriptionManager)subscriptionManager;
                });
            }
        }
    }
}
