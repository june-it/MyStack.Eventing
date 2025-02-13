using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing
{
    public interface IEventingBuilder
    {
        IServiceCollection Services { get; }
        public Assembly[] Assemblies { get; }
    }
}
