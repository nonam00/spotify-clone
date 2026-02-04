using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQueryHandler :
    IQueryHandler<GetLikedSongListForPlaylistQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetLikedSongListForPlaylistQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetLikedSongListForPlaylistQuery request, CancellationToken cancellationToken)
    {
        var likedNotInPlaylist = await _songsRepository
            .GetLikedByUserIdExcludeInPlaylist(request.UserId, request.PlaylistId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<SongListVm>.Success(new SongListVm(likedNotInPlaylist));
    }
}