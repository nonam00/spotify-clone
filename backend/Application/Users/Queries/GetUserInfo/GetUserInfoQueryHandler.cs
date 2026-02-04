using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, Result<UserInfo>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<GetUserInfoQueryHandler> _logger;
    
    public GetUserInfoQueryHandler(IUsersRepository usersRepository, ILogger<GetUserInfoQueryHandler> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<Result<UserInfo>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            _logger.LogWarning("Tried to get info for non-existing user {UserId}.", request.UserId);
            return Result<UserInfo>.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("User {UserId} tried to get their info but user is not active.", request.UserId);
            return Result<UserInfo>.Failure(UserDomainErrors.NotActive);
        }
        
        return Result<UserInfo>.Success(new UserInfo(user.Email, user.FullName, user.AvatarPath));
    }
}