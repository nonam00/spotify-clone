using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistDeleted;

public class PlaylistDeletedEventHandler : IDomainEventHandler<PlaylistDeletedEvent>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlaylistDeletedEventHandler> _logger;

    public PlaylistDeletedEventHandler(HttpClient httpClient, ILogger<PlaylistDeletedEventHandler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task HandleAsync(PlaylistDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Playlist deleted: {PlaylistId}", @event.PlaylistId);

        await _httpClient.DeleteAsync(
            "http://nginx/files/api/v1?type=image&file_id=" + @event.ImagePath,
            cancellationToken);
    }
}