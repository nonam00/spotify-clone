using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public record GetUserInfoQuery(Guid UserId) : IQuery<Result<UserInfo>>;