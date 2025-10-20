using Application.LikedSongs.Interfaces;
using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQueryHandler
    : IQueryHandler<GetLikedSongListForPlaylistQuery, LikedSongListVm>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public GetLikedSongListForPlaylistQueryHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task<LikedSongListVm> Handle(GetLikedSongListForPlaylistQuery request,
        CancellationToken cancellationToken)
    {
        var liked = await _likedSongsRepository.GetListForPlaylist(
            request.UserId, request.PlaylistId, cancellationToken);
        
        return new LikedSongListVm { LikedSongs = liked };
    }
}