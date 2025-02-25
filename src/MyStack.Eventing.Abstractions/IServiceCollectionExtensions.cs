using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds event support.
        /// </summary>
        /// <param name="services">The dependency service collection interface.</param>
        /// <param name="configure">Configures the events.</param>
        /// <param name="assemblies">The collection of assemblies to register subscription services.</param>
        /// <returns></returns>
        public static IServiceCollection AddEventing(this IServiceCollection services, Action<IEventingBuilder> configure, params Assembly[] assemblies)
        {
            var eventingBuilder = new EventingBuilder(services, assemblies);
            configure(eventingBuilder);
            services.AddTransient<IEventingBuilder>(factory =>
            {
                return eventingBuilder;
            });
            var eventHandlerTypes = assemblies.SelectMany(x => x.GetTypes().Where(x => !x.IsAbstract && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
            if (eventHandlerTypes.Any())
            {
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var handlerInterfaces = eventHandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        services.AddTransient(typeof(IEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                    }
                }
            }
            return services;
        }
    }
}
