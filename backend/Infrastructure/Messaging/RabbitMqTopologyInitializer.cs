using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

using Infrastructure.Email;
using Infrastructure.Files;
using Infrastructure.Transcription;

namespace Infrastructure.Messaging;

public sealed class RabbitMqTopologyInitializer
{
    private readonly RabbitMqConnectionProvider _connectionProvider;
    private readonly ILogger<RabbitMqTopologyInitializer> _logger;
    private const string GlobalDlxName = "system.dlx";

    public RabbitMqTopologyInitializer(
        RabbitMqConnectionProvider connectionProvider,
        ILogger<RabbitMqTopologyInitializer> logger)
    {
        _connectionProvider = connectionProvider;
        _logger = logger;
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Initializing RabbitMQ Topology");
        
        var connection = await _connectionProvider.GetConnectionAsync().ConfigureAwait(false);
        
        await using var channel = await connection
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await DeclareSystemDlxAsync(channel, cancellationToken).ConfigureAwait(false);
        
        IEnumerable<Task> tasks =
        [
            DeclareFileServiceTopologyAsync(channel, cancellationToken),
            DeclareEmailServiceTopologyAsync(channel, cancellationToken),
            DeclareTranscriptionServiceTopologyAsync(channel, cancellationToken),
        ];
        await Task.WhenAll(tasks).ConfigureAwait(false);
        
        _logger.LogTrace("Finished initializing RabbitMQ Topology");
    }
    
    private static async Task DeclareSystemDlxAsync(IChannel channel, CancellationToken cancellationToken)
    {
        // DLX
        await channel.ExchangeDeclareAsync(
            exchange: GlobalDlxName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    private static async Task DeclareFileServiceTopologyAsync(
        IChannel channel, CancellationToken cancellationToken = default)
    {
        const string mainQueue = FileServiceMessaging.DeleteFileQueue;
        const string dlqQueue = $"{mainQueue}.dlq";
        
        // DLQ
        await channel.QueueDeclareAsync(
            queue: dlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: dlqQueue,
            exchange: GlobalDlxName,
            routingKey: dlqQueue,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // Main exchange
        await channel.ExchangeDeclareAsync(
            exchange: FileServiceMessaging.FileExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // DLQ arguments
        var args = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", GlobalDlxName },
            { "x-dead-letter-routing-key", dlqQueue }
        };

        // Queues must be declared in consumer service
        await channel.QueueDeclareAsync(
            queue: FileServiceMessaging.DeleteFileQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args,
            noWait: false,
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

    private static async Task DeclareEmailServiceTopologyAsync(
        IChannel channel, CancellationToken cancellationToken = default)
    {
        const string mainQueue = EmailServiceContract.SendEmailQueue;
        const string dlqQueue = $"{mainQueue}.dlq";
        
        // DLQ
        await channel.QueueDeclareAsync(
            queue: dlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: dlqQueue,
            exchange: GlobalDlxName,
            routingKey: dlqQueue,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // Main exchange
        await channel.ExchangeDeclareAsync(
            exchange: EmailServiceContract.EmailExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // DLQ arguments
        var args = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", GlobalDlxName },
            { "x-dead-letter-routing-key", dlqQueue }
        };

        // Queues must be declared in consumer service
        await channel.QueueDeclareAsync(
            queue: EmailServiceContract.SendEmailQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args,
            noWait: false,
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

    private static async Task DeclareTranscriptionServiceTopologyAsync(
        IChannel channel, CancellationToken cancellationToken = default)
    {
        const string transcribeSongQueue = TranscriptionServiceMessaging.TranscribeSongQueue;
        const string transcribeSongDlq = $"{transcribeSongQueue}.dlq";
        
        const string updateSongInformationQueue = TranscriptionServiceMessaging.UpdateSongInformationQueue;
        const string updateSongInformationDlq = $"{updateSongInformationQueue}.dlq";
        
        // DLQ
        await channel.QueueDeclareAsync(
            queue: transcribeSongDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: transcribeSongDlq,
            exchange: GlobalDlxName,
            routingKey: transcribeSongDlq,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueDeclareAsync(
            queue: updateSongInformationDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: updateSongInformationDlq,
            exchange: GlobalDlxName,
            routingKey: updateSongInformationDlq,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // Main exchange
        await channel.ExchangeDeclareAsync(
            exchange: TranscriptionServiceMessaging.TranscriptionExchange, 
            type: ExchangeType.Topic, 
            durable: true, 
            autoDelete: false, 
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // DLQ arguments
        var transcribeSongArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", GlobalDlxName },
            { "x-dead-letter-routing-key", transcribeSongDlq }
        };

        var updateSongInformationArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", GlobalDlxName },
            { "x-dead-letter-routing-key", updateSongInformationDlq }
        };

        // Queues must be declared in consumer service
        await channel.QueueDeclareAsync(
            queue: TranscriptionServiceMessaging.TranscribeSongQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: transcribeSongArgs,
            noWait: false,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
            queue: TranscriptionServiceMessaging.TranscribeSongQueue,
            exchange: TranscriptionServiceMessaging.TranscriptionExchange,
            routingKey: TranscriptionServiceMessaging.TranscribeSongRoutingKey,
            arguments: null,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueDeclareAsync(
                queue: TranscriptionServiceMessaging.UpdateSongInformationQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: updateSongInformationArgs,
                noWait: false,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        await channel.QueueBindAsync(
                queue: TranscriptionServiceMessaging.UpdateSongInformationQueue,
                exchange: TranscriptionServiceMessaging.TranscriptionExchange,
                routingKey: TranscriptionServiceMessaging.UpdateSongInformationRoutingKey,
                arguments: null,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}