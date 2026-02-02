using Domain.Common;
using Domain.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanUserRefreshTokens;

public class CleanUserRefreshTokensCommandHandler : ICommandHandler<CleanUserRefreshTokensCommand, Result>
{
    private readonly IUsersRepository _usersRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public CleanUserRefreshTokensCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CleanUserRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithRefreshTokens(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var cleanRefreshTokensResult = user.CleanRefreshTokens();
        if (cleanRefreshTokensResult.IsFailure)
        {
            return cleanRefreshTokensResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}