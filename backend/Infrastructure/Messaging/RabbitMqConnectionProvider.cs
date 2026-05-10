using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

public class RabbitMqConnectionProvider : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options)
    {
        _factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            Port = options.Value.Port,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            TopologyRecoveryEnabled = true
        };
    }

    // It is not allowed to wrap received connection to using block because it is causing shared connection to close and it must be recreated
    public async ValueTask<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
        try
        {
            // Double-Check Locking
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection is { IsOpen: false })
            {
                await _connection.DisposeAsync().ConfigureAwait(false);
                _connection = null;
            }

            _connection = await _factory.CreateConnectionAsync().ConfigureAwait(false);
            return _connection;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync().ConfigureAwait(false);
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
        _semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }
}