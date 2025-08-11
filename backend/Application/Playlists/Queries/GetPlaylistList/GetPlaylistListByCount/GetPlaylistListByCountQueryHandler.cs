using MediatR;

using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount;

public class GetPlaylistListByCountQueryHandler
    : IRequestHandler<GetPlaylistListByCountQuery, PlaylistListVm>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public GetPlaylistListByCountQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<PlaylistListVm> Handle(
        GetPlaylistListByCountQuery request,
        CancellationToken cancellationToken)
    {
        var playlists = await _playlistsRepository.TakeList(
            request.UserId, request.Count, cancellationToken);

        return new PlaylistListVm { Playlists = playlists };
    }
}