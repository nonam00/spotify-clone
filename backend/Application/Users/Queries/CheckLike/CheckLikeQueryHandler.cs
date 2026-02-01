using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Queries.CheckLike;

public class CheckLikeQueryHandler : IQueryHandler<CheckLikeQuery, Result<bool>>
{
    private readonly IUsersRepository _usersRepository;
    
    public CheckLikeQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<bool>> Handle(CheckLikeQuery request, CancellationToken cancellationToken)
    {
        var isLiked = await _usersRepository
            .CheckIfSongLiked(request.UserId, request.SongId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<bool>.Success(isLiked);
    }
}