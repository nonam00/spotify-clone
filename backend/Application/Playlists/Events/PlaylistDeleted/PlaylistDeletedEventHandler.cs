using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Interfaces;
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

    public Task HandleAsync(PlaylistDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling playlist {PlaylistId} deleted event", @event.PlaylistId);

        if (string.IsNullOrWhiteSpace(@event.ImagePath))
        {
            return Task.CompletedTask;
        }
        
        _logger.LogDebug(
            "Deleting playlist {PlaylistId} cover image {ImagePath}",
            @event.PlaylistId, @event.ImagePath);

        return _fileServiceClient.DeleteAsync(@event.ImagePath, "image", cancellationToken);

    }
}