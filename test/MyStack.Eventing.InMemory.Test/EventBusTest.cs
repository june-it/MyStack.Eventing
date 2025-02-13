using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyStack.Eventing.InMemory.Test
{
    public class EventBusTest
    {
        [Fact]
        public void Test1()
        {
            var services = new ServiceCollection();
            services.AddEventing(configure =>
            {
                configure.UseInMemory();
            }, Assembly.GetExecutingAssembly());
            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.PublishAsync(new FooEvent());
        }
    }
}