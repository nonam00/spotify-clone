using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandHandler : ICommandHandler<DeleteRefreshTokenCommand>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRefreshTokenCommandHandler(IRefreshTokensRepository refreshTokensRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValue(request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            throw new ArgumentException($"Refresh token {request.RefreshToken} doesn't exist");
        }
        
        _refreshTokensRepository.Delete(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}