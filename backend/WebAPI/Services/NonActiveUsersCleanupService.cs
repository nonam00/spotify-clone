using Application.Shared.Messaging;
using Application.Users.Commands.CleanupNonActiveUsers;

namespace WebAPI.Services;

public class NonActiveUsersCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NonActiveUsersCleanupService> _logger;

    public NonActiveUsersCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<NonActiveUsersCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Deletes users who have not confirmed their account or was disabled by moderators on service startup and then once in 48 hours
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork();
        
        using var timer = new PeriodicTimer(TimeSpan.FromHours(48));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWork();
        }
    }

    private async Task DoWork()
    {
        _logger.LogInformation("Starting non-active users cleanup");
        
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediator.Send(new CleanupNonActiveUsersCommand());
        
        _logger.LogInformation("Finished non-active users cleanup");
    }
}