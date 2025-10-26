using Application.Shared.Messaging;
using Application.Users.Commands.CleanupRefreshTokens;

namespace WebAPI.Services;

public class RefreshTokensCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RefreshTokensCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new CleanupRefreshTokensCommand());
    }
}