using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public Email Email { get; init; }

    public UserRegisteredEvent(Guid userId, Email email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserProfileUpdatedEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string FullName { get; init; }
    public FilePath NewAvatarPath { get; init; }
    public FilePath OldAvatarPath { get; init; }
    
    public UserProfileUpdatedEvent(Guid userId, string fullName, FilePath newAvatarPath, FilePath oldAvatarPath)
    {
        UserId = userId;
        FullName = fullName;
        NewAvatarPath = newAvatarPath;
        OldAvatarPath = oldAvatarPath;
    }
}