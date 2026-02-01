using Application.Shared.Messaging;
using Application.Users.Models;
using Domain.Common;

namespace Application.Users.Queries.GetUserInfo;

public record GetUserInfoQuery(Guid UserId) : IQuery<Result<UserInfo>>;