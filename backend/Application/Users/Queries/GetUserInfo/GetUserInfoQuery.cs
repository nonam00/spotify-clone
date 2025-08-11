using MediatR;

using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQuery : IRequest<UserInfo>
{
    public Guid UserId { get; init; } 
}