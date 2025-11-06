using Microsoft.Extensions.Logging;

using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.AddSongToPlaylist;

public class AddSongToPlaylistCommandHandler : ICommandHandler<AddSongToPlaylistCommand>
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

    public async Task Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);

        if (playlist == null)
        {
            _logger.LogError(
                "User {userId} tried to add song {songId} to playlist {playlistId} but playlist does not exist",
                request.UserId, request.SongId, request.PlaylistId);
            throw new ArgumentException("Playlist does not exist.");
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {userId} tried to add song {songId} to playlist {playlistId} that belongs to user {ownerId}",
                request.UserId, request.SongId, request.PlaylistId, playlist.UserId);
            throw new OwnershipException("You do not have permission to add songs to this playlist.");
        }

        if (!playlist.AddSong(request.SongId))
        {
            _logger.LogError(
                "User {userId} tried to add song {songId} that is already in playlist {playlistId}",
                request.UserId, request.SongId, playlist.Id);
            throw new Exception("Song already added to playlist.");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}