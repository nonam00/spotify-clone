using System.Text.Json;
using RabbitMQ.Client;

using Application.Shared.Messaging;

namespace Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly RabbitMqConnectionProvider _connectionProvider;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    
    public RabbitMqMessagePublisher(RabbitMqConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionProvider.GetConnectionAsync().ConfigureAwait(false);
        
        await using var channel = await connection
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        var jsonBody = JsonSerializer.SerializeToUtf8Bytes(message, JsonSerializerOptions);
        
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };
        
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: jsonBody,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}