using Application.Shared.Messaging;
using Application.Playlists.Interfaces;
using Application.Playlists.Models;
using Microsoft.Extensions.Logging;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryHandler : IQueryHandler<GetPlaylistByIdQuery, PlaylistVm>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly ILogger<GetPlaylistByIdQueryHandler> _logger;

    public GetPlaylistByIdQueryHandler(
        IPlaylistsRepository playlistsRepository,
        ILogger<GetPlaylistByIdQueryHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _logger = logger;
    }

    public async Task<PlaylistVm> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.PlaylistId, cancellationToken);
        
        if (playlist != null)
        {
            return new PlaylistVm(playlist.Id, playlist.Title, playlist.Description, playlist.ImagePath);
        }
        
        _logger.LogError("Tried to get playlist {playlistId} but it does not exist", request.PlaylistId);
        throw new ArgumentException($"Playlist does not exist");
    }
}