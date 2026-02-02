using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Playlists.Commands.AddSongsToPlaylist;

public class AddSongToPlaylistCommandHandler : ICommandHandler<AddSongsToPlaylistCommand, Result>
{
    private readonly IPlaylistsRepository _playlistsRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddSongToPlaylistCommandHandler> _logger;
    
    public AddSongToPlaylistCommandHandler(
        IPlaylistsRepository playlistsRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork, 
        ILogger<AddSongToPlaylistCommandHandler> logger)
    {
        _playlistsRepository = playlistsRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddSongsToPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);

        if (playlist is null)
        {
            _logger.LogError(
                "User {UserId} tried to add songs to playlist {PlaylistId} but playlist does not exist",
                request.UserId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {UserId} tried to add songs to playlist {PlaylistId} that belongs to user {OwnerId}",
                request.UserId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }
        
        var songs = await _songsRepository.GetListByIds(request.SongIds, cancellationToken);

        if (songs.Count != request.SongIds.Count)
        {
            _logger.LogError(
                "User {UserId} tried to add songs to playlist {PlaylistId} but some songs does not exist",
                request.UserId, request.PlaylistId);
            return Result.Failure(SongErrors.SongsNotFound);
        }

        var addSongsResult = playlist.AddSongs(songs);
        if (addSongsResult.IsFailure)
        {
            switch (addSongsResult.Error.Code)
            {
                case nameof(PlaylistDomainErrors.AlreadyContainsSong):
                    _logger.LogError(
                        "User {UserId} tried to add songs that is already in playlist {PlaylistId}",
                        request.UserId, playlist.Id);
                    break;
                case nameof(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong):
                    _logger.LogError(
                        "User {UserId} tried to add unpublished songs to playlist {PlaylistId}",
                        request.UserId, playlist.Id);
                    break;
                default:
                    _logger.LogError(
                        "User {UserId} tried to add songs to playlist {PlaylistId}" +
                        " but domain error occurred: {DomainErrorDescription}",
                        request.UserId, playlist.Id, addSongsResult.Error.Description);
                    break;
            }

            return addSongsResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}