using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Domain.Models;

namespace Application.Playlists.Commands.RemoveSongFromPlaylist;

public class RemoveSongFromPlaylistCommandHandler: ICommandHandler<RemoveSongFromPlaylistCommand, Result>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveSongFromPlaylistCommandHandler> _logger;

    public RemoveSongFromPlaylistCommandHandler(
        IPlaylistsRepository playlistsRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveSongFromPlaylistCommandHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveSongFromPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);
        
        if (playlist is null)
        {
            _logger.LogError(
                "User {UserId} tried to remove song {SongId} from playlist {PlaylistId} but playlist does not exist",
                request.UserId, request.SongId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {UserId} tried to remove {SongId} from playlist {PlaylistId} that belongs to user {OwnerId}",
                request.UserId, request.SongId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }

        var removeSongsResult = playlist.RemoveSong(request.SongId);
        if (removeSongsResult.IsFailure)
        {
            if (removeSongsResult.Error.Code == nameof(PlaylistDomainErrors.DoesntContainSong))
            {
                _logger.LogError(
                    "User {UserId} tried to remove song {SongId} but it's not in playlist {PlaylistId}",
                    request.UserId, request.SongId, playlist.Id);        
            }
            else
            {
                _logger.LogError(
                    "User {UserId} tried to remove song {SongId} from playlist {PlaylistId}" +
                    " but domain error occurred: {DomainErrorDescription}",
                    request.UserId, request.SongId, playlist.Id, removeSongsResult.Error.Description);
            }
            return removeSongsResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}