using Microsoft.Extensions.Logging;

using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.AddSongsToPlaylist;

public class AddSongToPlaylistCommandHandler : ICommandHandler<AddSongsToPlaylistCommand, Result>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddSongToPlaylistCommandHandler> _logger;
    
    public AddSongToPlaylistCommandHandler(
        IPlaylistsRepository playlistsRepository,
        IUnitOfWork unitOfWork, 
        ILogger<AddSongToPlaylistCommandHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddSongsToPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);

        if (playlist == null)
        {
            _logger.LogError(
                "User {userId} tried to add songs to playlist {playlistId} but playlist does not exist",
                request.UserId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {userId} tried to add songs to playlist {playlistId} that belongs to user {ownerId}",
                request.UserId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }

        foreach (var songId in request.SongIds)
        {
            if (!playlist.AddSong(songId))
            {
                _logger.LogError(
                    "User {userId} tried to add song {songId} that is already in playlist {playlistId}",
                    request.UserId, songId, playlist.Id);
                return Result.Failure(PlaylistErrors.SongAlreadyInPlaylist);
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}