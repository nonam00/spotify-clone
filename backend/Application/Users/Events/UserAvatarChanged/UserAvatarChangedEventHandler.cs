using Domain.Events;
using Application.Shared.Integration;
using Application.Shared.Messaging;

namespace Application.Users.Events.UserAvatarChanged;

public class UserAvatarChangedEventHandler : IDomainEventHandler<UserAvatarChangedEvent>
{ 
    private readonly IFileServicePublisher _fileServicePublisher;

    public UserAvatarChangedEventHandler(IFileServicePublisher fileServicePublisher)
    {
        _fileServicePublisher = fileServicePublisher;
    }

    public Task HandleAsync(UserAvatarChangedEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@event.OldAvatarPath))
        {
            return Task.CompletedTask;
        }
        return _fileServicePublisher.PublishDeleteFileAsync(@event.OldAvatarPath, "image", cancellationToken);
    }
}