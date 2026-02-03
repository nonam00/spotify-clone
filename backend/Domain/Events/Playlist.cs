using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class PlaylistImageChangedEvent(Guid playlistId, FilePath oldImagePath) : DomainEvent
{
    public Guid PlaylistId { get; } = playlistId;
    public FilePath OldImagePath { get; } = oldImagePath;
}

public class PlaylistDeletedEvent(Guid playlistId, FilePath imagePath) : DomainEvent
{
    public Guid PlaylistId { get; } = playlistId;
    public FilePath ImagePath { get; } = imagePath;
}