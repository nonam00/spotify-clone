using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public class SongDeletedEvent : DomainEvent
{
    public Guid SongId { get; private set; }
    public FilePath Image { get; private set; }
    public FilePath Audio { get; private set; }
    
    public SongDeletedEvent(Guid id, FilePath image, FilePath audio)
    {
        SongId = id;
        Image = image;
        Audio = audio;
    }
}