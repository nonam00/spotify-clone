using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistDetailsUpdated;

public class PlaylistDetailsUpdatedEventHandler : IDomainEventHandler<PlaylistDetailsUpdatedEvent>
{ 
    private readonly IFileServiceClient _fileServiceClient;
    private readonly ILogger<PlaylistDetailsUpdatedEventHandler> _logger;

    public PlaylistDetailsUpdatedEventHandler(
        IFileServiceClient fileServiceClient,
        ILogger<PlaylistDetailsUpdatedEventHandler> logger)
    {
        _fileServiceClient = fileServiceClient;
        _logger = logger;
    }

    public async Task HandleAsync(PlaylistDetailsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling playlist {playlistId} details updated event",  @event.PlaylistId);

        if (!string.IsNullOrWhiteSpace(@event.OldImagePath) && @event.OldImagePath != @event.NewImagePath)
        {
            _logger.LogDebug("Deleting playlist {playlistId} old cover image {imagePath}",
                @event.PlaylistId, @event.OldImagePath.Value);
            
            await _fileServiceClient
                .DeleteAsync(@event.OldImagePath, "image", cancellationToken)
                .ConfigureAwait(false);
        }
    }
}