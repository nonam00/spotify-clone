using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Playlists.Commands.AddSongToPlaylist;

public class AddSongToPlaylistCommandHandler : ICommandHandler<AddSongToPlaylistCommand, Result>
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

    public async Task<Result> Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);

        if (playlist is null)
        {
            _logger.LogError(
                "User {UserId} tried to add song {SongId} to playlist {PlaylistId} but playlist does not exist.",
                request.UserId, request.SongId, request.PlaylistId);
            return Result.Failure(PlaylistErrors.NotFound);
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {UserId} tried to add song {SongId} to playlist {PlaylistId} that belongs to user {OwnerId}.",
                request.UserId, request.SongId, request.PlaylistId, playlist.UserId);
            return Result.Failure(PlaylistErrors.OwnershipError);
        }
        
        var song = await _songsRepository.GetById(request.SongId, cancellationToken);
        
        if (song is null)
        {
            _logger.LogError(
                "User {UserId} tried to add song {SongId} to playlist {PlaylistId} but song does not exist.",
                request.UserId, request.SongId, request.PlaylistId);
            return Result.Failure(SongErrors.NotFound);
        }

        var addSongResult = playlist.AddSong(song);
        if (addSongResult.IsFailure)
        {
            switch (addSongResult.Error.Code)
            {
                case nameof(PlaylistDomainErrors.AlreadyContainsSong):
                    _logger.LogError(
                        "User {UserId} tried to add song {SongId} that is already in playlist {PlaylistId}.",
                        request.UserId, request.SongId, playlist.Id);
                    break;
                case nameof(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong):
                    _logger.LogError(
                        "User {UserId} tried to add unpublished song {SongId} to playlist {PlaylistId}.",
                        request.UserId, request.SongId, playlist.Id);
                    break;
                default:
                    _logger.LogError(
                        "User {UserId} tried to add song {SongId} to playlist {PlaylistId}" +
                        " but domain error occurred:\n{DomainErrorDescription}",
                        request.UserId, request.SongId, playlist.Id, addSongResult.Error.Description);
                    break;
            }
            return addSongResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "User {UserId} successfully added song {SongId} to playlist {PlaylistId}.",
            request.UserId, request.SongId, playlist.Id);

        return Result.Success();
    }
}