using Application.Shared.Messaging;
using Application.Songs.Commands.DeleteMarkedForDeletionSongs;

namespace WebAPI.Services;

public class MarkedForDeletionSongsCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MarkedForDeletionSongsCleanupService> _logger;

    public MarkedForDeletionSongsCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<MarkedForDeletionSongsCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Deletes songs that was marked for deletion by moderator on service startup and then every hour
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork();
        
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWork();
        }
    }

    private async Task DoWork()
    {
        _logger.LogInformation("Starting marked for deletion songs cleanup");
        
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediator.Send(new DeleteMarkedForDeletionSongsCommand());
        
        _logger.LogInformation("Finished marked for deletion songs cleanup");
    }
}