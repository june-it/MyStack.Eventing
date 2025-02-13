using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing.InMemory
{
    public static class EventingBuilderExtensions
    {
        public static void UseInMemory(this IEventingBuilder builder)
        {
            builder.Services.AddTransient<IEventBus, InMemoryEventBus>();
        }
    }
}
