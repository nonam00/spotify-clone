using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandHandler : ICommandHandler<DeleteRefreshTokenCommand, Result>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRefreshTokenCommandHandler(IRefreshTokensRepository refreshTokensRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValue(request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            return Result.Failure(RefreshTokenErrors.NotFound);
        }
        
        _refreshTokensRepository.Delete(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}