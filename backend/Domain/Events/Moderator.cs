using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class ModeratorDeletedSongEvent(Guid id, FilePath image, FilePath audio) : DomainEvent
{
    public Guid SongId { get; } = id;
    public FilePath Image { get; } = image;
    public FilePath Audio { get; } = audio;
}

public class ModeratorDeactivatedUserEvent(Guid userId, FilePath avatarPath) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public FilePath AvatarPath { get; } = avatarPath;
}