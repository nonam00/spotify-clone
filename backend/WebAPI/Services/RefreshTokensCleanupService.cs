using MediatR;

using Application.Users.Commands.CleanupRefreshTokens;

namespace WebAPI.Services;

public class RefreshTokensCleanupService : BackgroundService
{
    private readonly IMediator _mediator;

    public RefreshTokensCleanupService(IMediator mediator)
    {
        _mediator = mediator;
    }

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
        var command = new CleanupRefreshTokensCommand();
        await _mediator.Send(command);
    }
}