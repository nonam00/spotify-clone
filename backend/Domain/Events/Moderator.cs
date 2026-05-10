using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public record ModeratorDeletedSongEvent(Guid SongId, FilePath ImagePath, FilePath AudioPath) : DomainEvent;

public record ModeratorDeactivatedUserEvent(Guid UserId, FilePath AvatarPath) : DomainEvent;