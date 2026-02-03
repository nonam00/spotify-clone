using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanUserRefreshTokens;

public class CleanUserRefreshTokensCommandHandler : ICommandHandler<CleanUserRefreshTokensCommand, Result>
{
    private readonly IUsersRepository _usersRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CleanUserRefreshTokensCommandHandler> _logger;

    public CleanUserRefreshTokensCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<CleanUserRefreshTokensCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CleanUserRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithRefreshTokens(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Tried to clean user {UserId} refresh tokens but user doesnt exist", request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError(
                "User {UserId} tried to clean their refresh tokens but user is not active.",
                request.UserId);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var cleanRefreshTokensResult = user.CleanRefreshTokens();
        if (cleanRefreshTokensResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to clean their refresh tokens but domain error occurred:\n{DomainErrorDescription}.",
                request.UserId, cleanRefreshTokensResult.Error.Description);
            return cleanRefreshTokensResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully cleaned their refresh tokens.", request.UserId);
        
        return Result.Success();
    }
}