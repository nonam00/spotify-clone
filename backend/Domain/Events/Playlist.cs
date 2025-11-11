using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class PlaylistDetailsUpdatedEvent : DomainEvent
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public FilePath NewImagePath { get; init; }
    public FilePath OldImagePath { get; init; }

    public PlaylistDetailsUpdatedEvent(
        Guid playlistId, Guid userId, string title, string description, FilePath newImagePath, FilePath oldImagePath)
    {
        PlaylistId = playlistId;
        UserId = userId;
        Title = title;
        Description = description;
        NewImagePath = newImagePath;
        OldImagePath = oldImagePath;
    }
}

public class PlaylistDeletedEvent : DomainEvent
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; init; }
    public FilePath ImagePath { get; init; }

    public PlaylistDeletedEvent(Guid playlistId, Guid userId, FilePath imagePath)
    {
        PlaylistId = playlistId;
        UserId = userId;
        ImagePath = imagePath;
    }
}