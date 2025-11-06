using Microsoft.Extensions.Logging;

using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.RemoveSongFromPlaylist;

public class RemoveSongFromPlaylistCommandHandler: ICommandHandler<RemoveSongFromPlaylistCommand>
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

    public async Task Handle(RemoveSongFromPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(request.PlaylistId, cancellationToken);
        
        if (playlist == null)
        {
            _logger.LogError(
                "User {userId} tried to remove song {songId} from playlist {playlistId} but playlist does not exist",
                request.UserId, request.SongId, request.PlaylistId);
            throw new ArgumentException("Playlist does not exist.");
        }
        
        if (playlist.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User {userId} tried to add remove {songId} from playlist {playlist} that belongs to user {ownerId}",
                request.UserId, request.SongId, request.PlaylistId, playlist.UserId);
            throw new OwnershipException("You do not have permission to remove songs from this playlist");
        }

        if (!playlist.RemoveSong(request.SongId))
        {
            _logger.LogError(
                "User {userId} tried to remove song {songId} but it's not in playlist {playlistId}",
                request.UserId, request.SongId, playlist.Id);        
            throw new Exception("Song not found in playlist");
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}