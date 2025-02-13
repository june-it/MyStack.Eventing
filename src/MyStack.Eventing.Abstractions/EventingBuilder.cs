using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing
{
    public class EventingBuilder : IEventingBuilder
    {
        public IServiceCollection Services { get; }
        public Assembly[] Assemblies { get; }
        public EventingBuilder(IServiceCollection services, Assembly[] assemblies)
        {
            Services = services;
            Assemblies = assemblies;
        }
    }
}
