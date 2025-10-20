using Application.Shared.Messaging;

using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, UserInfo>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserInfoQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetInfoById(request.UserId, cancellationToken);
        return user;
    }
}