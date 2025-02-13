using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MyStack.Eventing.RabbitMQ
{
    public class RabbitMQProvider(IOptions<RabbitMQOptions> options)
    {
        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);
        private readonly RabbitMQOptions _options = options.Value;

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            IConnection connection;
            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                ConnectionFactory factory = new()
                {
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost,
                    HostName = _options.HostName,
                    Port = _options.Port
                };
                connection = factory.CreateConnection();
            }
            finally
            {
                _connectionLock.Release();
            }
            return connection;
        }
    }
}
