using Microsoft.Extensions.Logging;

using Application.Shared.Messaging;
using Application.Shared.Data;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryHandler : IQueryHandler<GetPlaylistByIdQuery, Result<PlaylistVm>>
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

    public async Task<Result<PlaylistVm>> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetById(request.PlaylistId, cancellationToken).ConfigureAwait(false);
        
        if (playlist != null)
        {
            return Result<PlaylistVm>.Success(
                new PlaylistVm(playlist.Id, playlist.Title, playlist.Description, playlist.ImagePath));
        }
        
        _logger.LogError("Tried to get playlist {playlistId} but it does not exist", request.PlaylistId);
        return Result<PlaylistVm>.Failure(PlaylistErrors.NotFound);
    }
}