using Infrastructure.Files;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

public class RabbitMqConnectionProvider : IAsyncDisposable
{
    private readonly IConnectionFactory _factory;
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
        };
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _semaphoreSlim.WaitAsync();
        try
        {
            // Double-Check Locking
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            _connection = await _factory.CreateConnectionAsync();
            return _connection;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async Task InitializeInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await GetConnectionAsync();
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await channel.ExchangeDeclareAsync(
            exchange: FileServiceMessaging.FileExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken);

        await channel.QueueDeclarePassiveAsync(
            queue: FileServiceMessaging.DeleteFileQueue,
            cancellationToken: cancellationToken);
        
        await channel.QueueBindAsync(
            queue: FileServiceMessaging.DeleteFileQueue,
            exchange: FileServiceMessaging.FileExchange,
            routingKey: FileServiceMessaging.DeleteRoutingKey,
            arguments: null,
            cancellationToken: cancellationToken);
    }
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}