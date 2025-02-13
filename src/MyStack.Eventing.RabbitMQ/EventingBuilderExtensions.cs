using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing.RabbitMQ
{
    public static class EventingBuilderExtensions
    {
        public static void UseRabbitMQ(this IEventingBuilder builder, Action<RabbitMQOptions> configureRabbitMQ)
        {
            builder.Services.AddTransient<QueueBindValueProvider>();
            builder.Services.AddTransient<ExchangeDeclareValueProvider>();
            builder.Services.AddTransient<QueueDeclareValueProvider>();
            builder.Services.AddTransient<RabbitMQProvider>();
            builder.Services.AddTransient<IEventBus, RabbitMQEventBus>();
            builder.Services.Configure(configureRabbitMQ);
            builder.Services.AddTransient<SubscriptionManager>();
            if (builder.Assemblies.Length != 0)
                builder.Services.AddHostedService<RabbitMQBackgroundService>();
        }
    }
}
