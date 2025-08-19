using MediatR;

using Application.Users.Interfaces;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenQueryHandler : IRequestHandler<DeleteRefreshTokenQuery>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;

    public DeleteRefreshTokenQueryHandler(IRefreshTokensRepository refreshTokensRepository)
    {
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task Handle(DeleteRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValue(request.RefreshToken, cancellationToken);
        await _refreshTokensRepository.Delete(refreshToken, cancellationToken);
    }
}