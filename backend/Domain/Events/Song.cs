using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class SongDeletedEvent : DomainEvent
{
    public Guid SongId { get; }
    public FilePath Image { get; }
    public FilePath Audio { get; }
    
    public SongDeletedEvent(Guid id, FilePath image, FilePath audio)
    {
        SongId = id;
        Image = image;
        Audio = audio;
    }
}