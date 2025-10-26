using Application.Shared.Messaging;
using Application.Users.Commands.CleanupNonActiveUsers;

namespace WebAPI.Services;

public class NonActiveUsersCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public NonActiveUsersCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    // Deletes users who have not confirmed their account within an hour on service startup and then every hour
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
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new CleanupNonActiveUsersCommand());
    }
}