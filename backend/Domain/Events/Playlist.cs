using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class PlaylistImageChangedEvent : DomainEvent
{
    public Guid PlaylistId { get; }
    public FilePath NewImagePath { get; }
    public FilePath OldImagePath { get; }

    public PlaylistImageChangedEvent(Guid playlistId, FilePath newImagePath, FilePath oldImagePath)
    {
        PlaylistId = playlistId;
        NewImagePath = newImagePath;
        OldImagePath = oldImagePath;
    }
}

public class PlaylistDeletedEvent : DomainEvent
{
    public Guid PlaylistId { get; }
    public FilePath ImagePath { get; }

    public PlaylistDeletedEvent(Guid playlistId, FilePath imagePath)
    {
        PlaylistId = playlistId;
        ImagePath = imagePath;
    }
}