using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyStack.Eventing.RabbitMQ.Consumer
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
                   },
                   Assembly.GetExecutingAssembly());
               });

            var app = builder.Build();
            app.Run();
        }
    }
}
