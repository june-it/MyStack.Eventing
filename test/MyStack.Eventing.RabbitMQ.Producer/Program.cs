using MyStack.Eventing.RabbitMQ.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                       });
                   });
               });

            var app = builder.Build();

            var eventBus = app.Services.GetRequiredService<IEventBus>();
            var ev = new HelloMessage();
            ev.Meta.AddKeyValue("tenantid", "1234565");
            eventBus.PublishAsync(ev);

            app.Run();
        }
    }
}
