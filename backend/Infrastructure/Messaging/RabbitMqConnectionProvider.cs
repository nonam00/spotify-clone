using Microsoft.Extensions.Options;
using RabbitMQ.Client;

using Infrastructure.Email;
using Infrastructure.Files;

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

            _connection = await _factory.CreateConnectionAsync().ConfigureAwait(false);
            return _connection;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async Task InitializeInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await GetConnectionAsync().ConfigureAwait(false);
        await using var channel = await connection
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        IEnumerable<Task> tasks =
        [
            DeclareFileServiceInfrastructureAsync(channel, cancellationToken),
            DeclareEmailServiceInfrastructureAsync(channel, cancellationToken)
        ];
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static async Task DeclareFileServiceInfrastructureAsync(
        IChannel channel, CancellationToken cancellationToken = default)
    {
        await channel.ExchangeDeclareAsync(
            exchange: FileServiceMessaging.FileExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // Queues must be declared in consumer service
        await channel.QueueDeclareAsync(
            queue: FileServiceMessaging.DeleteFileQueue,
            durable: true,
            autoDelete: false,
            exclusive: false,
            noWait: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: FileServiceMessaging.DeleteFileQueue,
            exchange: FileServiceMessaging.FileExchange,
            routingKey: FileServiceMessaging.DeleteRoutingKey,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task DeclareEmailServiceInfrastructureAsync(
        IChannel channel, CancellationToken cancellationToken = default)
    {
        await channel.ExchangeDeclareAsync(
            exchange: EmailServiceContract.EmailExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // Queues must be declared in consumer service
        await channel.QueueDeclareAsync(
            queue: EmailServiceContract.SendEmailQueue,
            durable: true,
            autoDelete: false,
            exclusive: false,
            noWait: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: EmailServiceContract.SendEmailQueue,
            exchange: EmailServiceContract.EmailExchange,
            routingKey: EmailServiceContract.SendEmailRoutingKey,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
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