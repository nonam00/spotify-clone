using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public record UserRegisteredEvent(Guid UserId, Email Email) : DomainEvent;

public record UserAvatarChangedEvent(Guid UserId, FilePath OldAvatarPath) : DomainEvent;

public record UserUploadedSongEvent(Guid SongId, FilePath AudioPath) : DomainEvent;