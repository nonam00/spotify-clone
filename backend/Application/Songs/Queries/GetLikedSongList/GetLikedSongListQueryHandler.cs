using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongList;

public class GetLikedSongListQueryHandler : IQueryHandler<GetLikedSongListQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetLikedSongListQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetLikedSongListQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository
            .GetLikedByUserId(request.UserId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}