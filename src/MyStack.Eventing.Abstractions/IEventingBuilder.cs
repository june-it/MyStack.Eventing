using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing
{
    /// <summary>
    /// Represents the event building service.
    /// </summary>
    public interface IEventingBuilder
    {
        IServiceCollection Services { get; }
        public Assembly[] Assemblies { get; }
    }
}
