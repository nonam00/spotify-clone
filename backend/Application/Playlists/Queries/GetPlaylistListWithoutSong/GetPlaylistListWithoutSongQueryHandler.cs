using Application.Playlists.Interfaces;
using Application.Playlists.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Queries.GetPlaylistListWithoutSong;

public class GetPlaylistListWithoutSongQueryHandler
    : IQueryHandler<GetPlaylistListWithoutSongQuery, Result<PlaylistListVm>>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public GetPlaylistListWithoutSongQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<Result<PlaylistListVm>> Handle(
        GetPlaylistListWithoutSongQuery query, CancellationToken cancellationToken)
    {
        var playlists = await _playlistsRepository
            .GetListWithoutSong(query.UserId, query.SongId, cancellationToken)
            .ConfigureAwait(false);
        
        return Result<PlaylistListVm>.Success(new PlaylistListVm(playlists));
    }
}