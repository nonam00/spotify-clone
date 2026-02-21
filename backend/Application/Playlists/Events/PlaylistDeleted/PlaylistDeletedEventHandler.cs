using Domain.Events;
using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistDeleted;

public class PlaylistDeletedEventHandler : IDomainEventHandler<PlaylistDeletedEvent>
{
    private readonly IFileServicePublisher _fileServicePublisher;

    public PlaylistDeletedEventHandler(IFileServicePublisher fileServicePublisher) 
    {
        _fileServicePublisher = fileServicePublisher;
    }

    public Task HandleAsync(PlaylistDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@event.ImagePath))
        {
            return Task.CompletedTask;
        }
        return _fileServicePublisher.PublishDeleteFileAsync(@event.ImagePath, "image", cancellationToken);
    }
}