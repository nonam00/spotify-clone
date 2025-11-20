using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;
using Application.Users.Models;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUserList;

public class GetUserListQueryHandler : IQueryHandler<GetUserListQuery, Result<UserListVm>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<GetUserListQueryHandler> _logger;

    public GetUserListQueryHandler(
        IUsersRepository usersRepository,
        ISongsRepository songsRepository,
        ILogger<GetUserListQueryHandler> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<Result<UserListVm>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetListVm(cancellationToken);
        return Result<UserListVm>.Success(new UserListVm(users));
    }
}

