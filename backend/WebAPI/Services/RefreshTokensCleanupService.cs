using Application.Shared.Messaging;
using Application.Users.Commands.CleanupExpiredRefreshTokens;

namespace WebAPI.Services;

public class RefreshTokensCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokensCleanupService> _logger;

    public RefreshTokensCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokensCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Deletes outdated refresh tokens on service startup and then every 24 hours
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork();
        
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWork();
        }
    }

    private async Task DoWork()
    {
        _logger.LogInformation("Starting refresh tokens cleanup");
        
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        await mediator.Send(new CleanupExpiredRefreshTokensCommand());
        
        _logger.LogInformation("Finished refresh tokens cleanup");
    }
}