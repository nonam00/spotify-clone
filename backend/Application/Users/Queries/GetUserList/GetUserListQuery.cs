using Application.Shared.Messaging;
using Application.Users.Models;
using Domain.Common;

namespace Application.Users.Queries.GetUserList;

public record GetUserListQuery : IQuery<Result<UserListVm>>;

