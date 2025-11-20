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
        
        _logger.LogDebug("Deleting song {songId} image {image}",
            @event.SongId, @event.Image.Value);
        await _fileServiceClient.DeleteAsync(@event.Image, cancellationToken).ConfigureAwait(false);
        
        _logger.LogDebug("Deleting song {songId} audio {audio}",
            @event.SongId, @event.Audio.Value);
        await _fileServiceClient.DeleteAsync(@event.Audio, cancellationToken).ConfigureAwait(false);
    }
}