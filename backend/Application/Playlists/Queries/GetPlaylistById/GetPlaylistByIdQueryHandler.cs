using Application.Shared.Messaging;

using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryHandler : IQueryHandler<GetPlaylistByIdQuery, PlaylistVm>
{
    private readonly IPlaylistsRepository _playlistsRepository;

    public GetPlaylistByIdQueryHandler(IPlaylistsRepository playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }

    public async Task<PlaylistVm> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetVmById(request.PlaylistId, request.UserId, cancellationToken);
        
        return playlist;
    }
}