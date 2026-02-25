using Domain.Events;
using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Application.Playlists.Events.PlaylistImageChanged;

public class PlaylistImageChangedEventHandler : IDomainEventHandler<PlaylistImageChangedEvent>
{ 
    private readonly IFileServicePublisher _fileServicePublisher;
    
    public PlaylistImageChangedEventHandler(IFileServicePublisher fileServicePublisher)
    {
        _fileServicePublisher = fileServicePublisher;
    }

    public Task HandleAsync(PlaylistImageChangedEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@event.OldImagePath))
        {
            return Task.CompletedTask;
        }
        return _fileServicePublisher.PublishDeleteFileAsync(@event.OldImagePath, "image", cancellationToken);
    }
}