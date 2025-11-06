using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Queries.CheckLike;

public class CheckLikeQueryHandler : IQueryHandler<CheckLikeQuery, bool>
{
    private readonly IUsersRepository _usersRepository;
    
    public CheckLikeQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(CheckLikeQuery request, CancellationToken cancellationToken)
    {
        return await _usersRepository.CheckIfSongLiked(request.UserId, request.SongId, cancellationToken);
    }
}