using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public class ReorderSongsInPlaylistCommandHandler : ICommandHandler<ReorderSongsInPlaylistCommand, Result>
{
    private readonly IPlaylistsRepository  _playlistsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReorderSongsInPlaylistCommandHandler> _logger;

    public ReorderSongsInPlaylistCommandHandler(
        IPlaylistsRepository playlistsRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReorderSongsInPlaylistCommandHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ReorderSongsInPlaylistCommand command, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(command.PlaylistId, cancellationToken);

        if (playlist is null)
        {
            _logger.LogError(
                "User {UserId} tried to reorder songs in playlist {PlaylistId} but playlist does not exist.",
                command.UserId, command.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != command.UserId)
        {
            _logger.LogWarning(
                "User {UserId} tried to reorder songs in playlist {PlaylistId} that belongs to user {OwnerId}.",
                command.UserId, command.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }

        var reorderSongsResult = playlist.ReorderSongs(command.SongIds);
        if (reorderSongsResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to reorder songs in playlist {PlaylistId}" +
                " but domain error occurred:\n{DomainErrorDescription}",
                command.UserId, command.PlaylistId, reorderSongsResult.Error);
            return reorderSongsResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "User {UserId} successfully reordered songs in playlist {PlaylistId}.",
            command.UserId, playlist.Id);
        
        return Result.Success();
    }
}