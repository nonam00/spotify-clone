using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.CleanupRefreshTokens;

public class CleanupRefreshTokensCommandHandler : ICommandHandler<CleanupRefreshTokensCommand, Result>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CleanupRefreshTokensCommandHandler(IRefreshTokensRepository refreshTokensRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CleanupRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var expired = await _refreshTokensRepository.GetExpiredList(cancellationToken);
        
        if (expired.Count != 0)
        {
            _refreshTokensRepository.DeleteRange(expired);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        return Result.Success();
    }
}