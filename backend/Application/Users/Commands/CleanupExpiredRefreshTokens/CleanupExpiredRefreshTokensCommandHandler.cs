using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanupExpiredRefreshTokens;

public class CleanupExpiredRefreshTokensCommandHandler : ICommandHandler<CleanupExpiredRefreshTokensCommand, Result>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CleanupExpiredRefreshTokensCommandHandler(
        IRefreshTokensRepository refreshTokensRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CleanupExpiredRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var expired = await _refreshTokensRepository
            .GetExpiredList(cancellationToken)
            .ConfigureAwait(false);
        
        if (expired.Count != 0)
        {
            _refreshTokensRepository.DeleteRange(expired);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        
        return Result.Success();
    }
}