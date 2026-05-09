using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Application.Shared.Messaging;
using Application.Songs.Commands.UpdateTranscribeInformation;
using Infrastructure.Messaging;

namespace Infrastructure.Transcription;

public class UpdateSongInformationConsumer : BackgroundService
{
    private readonly RabbitMqConnectionProvider _connectionProvider;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateSongInformationConsumer> _logger;
    
    private IConnection? _connection;
    private IChannel? _channel;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public UpdateSongInformationConsumer(
        RabbitMqConnectionProvider connectionProvider,
        IServiceScopeFactory scopeFactory,
        ILogger<UpdateSongInformationConsumer> logger)
    {
        _connectionProvider = connectionProvider;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = await _connectionProvider.GetConnectionAsync().ConfigureAwait(false);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
        
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 100,
            global: false,
            cancellationToken: stoppingToken)
            .ConfigureAwait(false);

        await _channel.QueueDeclarePassiveAsync(
            queue: TranscriptionServiceMessaging.UpdateSongInformationQueue,
            cancellationToken: stoppingToken)
            .ConfigureAwait(false);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: TranscriptionServiceMessaging.UpdateSongInformationQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var deliveryTag = @event.DeliveryTag;
        
        var body = Encoding.UTF8.GetString(@event.Body.Span);
        try
        {
            var message = JsonSerializer.Deserialize<UpdateSongInformationMessage>(body, JsonSerializerOptions);
        
            if (message is null)
            {
                _logger.LogWarning("Received null or malformed message. DeliveryTag: {DeliveryTag}", deliveryTag);
                await _channel!.BasicRejectAsync(deliveryTag, requeue: false).ConfigureAwait(false);
                return;
            }

            var command = new UpdateTranscribeInformationCommand(
                message.SongId,
                message.ContainsExplicitContent,
                message.LyricsSegments);
            
            await mediator.Send(command);
            
            await _channel!.BasicAckAsync(deliveryTag, multiple: false).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while handling rabbitmq message: {ex}", ex);
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping UpdateSongInformation consumer...");

        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken).ConfigureAwait(false);
            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}