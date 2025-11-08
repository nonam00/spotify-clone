using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

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
        var songsInPlaylist = await _songsRepository.GetListByPlaylistId(request.PlaylistId, cancellationToken);
        var likedSongs = await _songsRepository.GetLikedByUserId(request.UserId, cancellationToken);
        var likedNotInPlaylist = likedSongs.Except(songsInPlaylist).ToList();
        return Result<SongListVm>.Success(new SongListVm(likedNotInPlaylist));
    }
}