using Domain.Events;
using Application.Shared.Integration;
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
        IEnumerable<Task> tasks =
        [
            _fileServicePublisher.PublishDeleteFileAsync(@event.ImagePath, "image", cancellationToken),
            _fileServicePublisher.PublishDeleteFileAsync(@event.AudioPath, "audio", cancellationToken)
        ];
        return Task.WhenAll(tasks);
    }
}