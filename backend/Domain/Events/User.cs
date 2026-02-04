using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class UserRegisteredEvent(Guid userId, Email email) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public Email Email { get; } = email;
}

public class UserAvatarChangedEvent(Guid userId, FilePath oldAvatarPath) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public FilePath OldAvatarPath { get; } = oldAvatarPath;
}