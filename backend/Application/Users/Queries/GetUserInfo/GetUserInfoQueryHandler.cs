using Microsoft.Extensions.Logging;

using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, UserInfo>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<GetUserInfoQueryHandler> _logger;
    
    public GetUserInfoQueryHandler(IUsersRepository usersRepository, ILogger<GetUserInfoQueryHandler> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (user != null)
        {
            return new UserInfo(user.Email, user.FullName, user.AvatarPath);
        }
        
        _logger.LogWarning("Tried to get info for non-existing user {userId}", request.UserId);
        throw new Exception("You are trying to get info for non-existing user");

    }
}