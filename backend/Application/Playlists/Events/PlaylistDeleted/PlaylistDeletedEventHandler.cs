using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Clients;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistDeleted;

public class PlaylistDeletedEventHandler : IDomainEventHandler<PlaylistDeletedEvent>
{
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<PlaylistDeletedEventHandler> _logger;

    public PlaylistDeletedEventHandler(IFileServiceClient fileServiceClient, ILogger<PlaylistDeletedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(PlaylistDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling playlist {playlistId} deleted event", @event.PlaylistId);
        _logger.LogDebug("Deleting playlist {playlistId} cover image {imagePath}",
            @event.PlaylistId, @event.ImagePath.Value);
        await _fileServiceClient.DeleteAsync(@event.ImagePath, cancellationToken).ConfigureAwait(false);
    }
}