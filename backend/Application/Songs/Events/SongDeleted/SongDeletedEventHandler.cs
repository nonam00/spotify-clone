using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Playlists.Events.PlaylistDeleted;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Songs.Events.SongDeleted;

public class SongDeletedEventHandler : IDomainEventHandler<SongDeletedEvent>
{
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<PlaylistDeletedEventHandler> _logger;

    public SongDeletedEventHandler(IFileServiceClient fileServiceClient, ILogger<PlaylistDeletedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(SongDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling song {songId} deleted event", @event.SongId);
        
        _logger.LogDebug("Deleting song {songId} image {image}", @event.SongId, @event.Image.Value);
        var deleteImageTask = _fileServiceClient.DeleteAsync(@event.Image, "image", cancellationToken);
        
        _logger.LogDebug("Deleting song {songId} audio {audio}", @event.SongId, @event.Audio.Value);
        var deleteAudioTask = _fileServiceClient.DeleteAsync(@event.Audio, "image", cancellationToken);
        
        await Task.WhenAll(deleteImageTask, deleteAudioTask).ConfigureAwait(false);
    }
}