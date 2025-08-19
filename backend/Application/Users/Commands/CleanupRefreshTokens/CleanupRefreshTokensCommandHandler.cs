using MediatR;

using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanupRefreshTokens;

public class CleanupRefreshTokensCommandHandler : IRequestHandler<CleanupRefreshTokensCommand>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;

    public CleanupRefreshTokensCommandHandler(IRefreshTokensRepository refreshTokensRepository)
    {
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task Handle(CleanupRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokensRepository.DeleteExpired(cancellationToken);
    }
}