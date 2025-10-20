using Application.Shared.Messaging;

using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQuery : IQuery<UserInfo>
{
    public Guid UserId { get; init; } 
}