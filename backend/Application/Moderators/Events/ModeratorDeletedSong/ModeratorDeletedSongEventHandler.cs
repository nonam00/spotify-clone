using Application.Shared.Integration;
using Domain.Events;
using Application.Shared.Messaging;

namespace Application.Moderators.Events.ModeratorDeletedSong;

public class ModeratorDeletedSongEventHandler : IDomainEventHandler<ModeratorDeletedSongEvent>
{
    private readonly IFileServicePublisher _fileServicePublisher;

    public ModeratorDeletedSongEventHandler(IFileServicePublisher fileServicePublisher)
    {
        _fileServicePublisher = fileServicePublisher;
    }

    public Task HandleAsync(ModeratorDeletedSongEvent @event, CancellationToken cancellationToken = default)
    {
        List<Task> tasks =
        [
            _fileServicePublisher.PublishDeleteFileAsync(@event.Image, "image", cancellationToken),
            _fileServicePublisher.PublishDeleteFileAsync(@event.Audio, "audio", cancellationToken)
        ];
        return Task.WhenAll(tasks);
    }
}