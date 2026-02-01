using Microsoft.Extensions.Logging;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Application.Users.Models;
using Domain.Common;

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

        if (user != null)
        {
            return Result<UserInfo>.Success(new UserInfo(user.Email, user.FullName, user.AvatarPath));
        }
        
        _logger.LogWarning("Tried to get info for non-existing user {userId}", request.UserId);
        return Result<UserInfo>.Failure(UserErrors.NotFound);
    }
}