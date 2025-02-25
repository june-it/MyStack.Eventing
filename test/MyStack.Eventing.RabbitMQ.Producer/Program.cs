using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.Eventing.RabbitMQ.Shared;

namespace MyStack.Eventing.RabbitMQ.Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureServices((context, services) =>
               {
                   services.AddLogging(logging =>
                   {
                       logging.AddConsole();
                   });
                   services.AddEventing(configure =>
                   {
                       configure.UseRabbitMQ(configureMQ =>
                       {
                           configureMQ.HostName = "120.77.76.98";
                           configureMQ.VirtualHost = "/";
                           configureMQ.Port = 25004;
                           configureMQ.UserName = "admin";
                           configureMQ.Password = "admin";
                           configureMQ.QueueOptions.Name = "MyStack";
                           configureMQ.ExchangeOptions.Name = "MyStack";
                           configureMQ.ExchangeOptions.ExchangeType = "topic";
                           configureMQ.RoutingKeyPrefix = "12345.";
                       });
                   });
               });

            var app = builder.Build();

            var eventBus = app.Services.GetRequiredService<IEventBus>();
            var ev = new HelloMessage();
            ev.Metadata.AddRabbitMQHeaders("tenantid", "1234565");
            eventBus.PublishAsync(ev);

            var ev2 = new HelloMessage2();
            ev2.Metadata.AddRabbitMQHeaders("tenantid", "1234565");
            eventBus.PublishAsync(ev2);

            var eventData = new EventData();
            eventBus.PublishAsync(eventData);

            app.Run();
        }
    }
}
