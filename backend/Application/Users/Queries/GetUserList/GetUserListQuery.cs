using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserList;

public record GetUserListQuery : IQuery<Result<UserListVm>>;

