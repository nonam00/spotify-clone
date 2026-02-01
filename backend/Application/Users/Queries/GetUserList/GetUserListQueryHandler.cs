using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Application.Users.Models;
using Domain.Common;

namespace Application.Users.Queries.GetUserList;

public class GetUserListQueryHandler : IQueryHandler<GetUserListQuery, Result<UserListVm>>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserListQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<UserListVm>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetListVm(cancellationToken).ConfigureAwait(false);
        return Result<UserListVm>.Success(new UserListVm(users));
    }
}

