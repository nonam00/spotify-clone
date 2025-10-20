using Application.Shared.Messaging;

using Application.Users.Interfaces;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandHandler : ICommandHandler<DeleteRefreshTokenCommand>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;

    public DeleteRefreshTokenCommandHandler(IRefreshTokensRepository refreshTokensRepository)
    {
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValue(request.RefreshToken, cancellationToken);
        await _refreshTokensRepository.Delete(refreshToken, cancellationToken);
    }
}