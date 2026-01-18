using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public Email Email { get; }

    public UserRegisteredEvent(Guid userId, Email email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserAvatarChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public FilePath NewAvatarPath { get; }
    public FilePath OldAvatarPath { get; }
    
    public UserAvatarChangedEvent(Guid userId, FilePath newAvatarPath, FilePath oldAvatarPath)
    {
        UserId = userId;
        NewAvatarPath = newAvatarPath;
        OldAvatarPath = oldAvatarPath;
    }
}