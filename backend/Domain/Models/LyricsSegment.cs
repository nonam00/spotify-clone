using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Models;

public class LyricsSegment: Entity<Guid>
{
    public Guid SongId { get; private init; }
    public LyricsSegmentData LyricsSegmentData { get; private init; } = null!;
    
    private LyricsSegment() { }

    public LyricsSegment(Guid songId, LyricsSegmentData lyricsSegmentData)
    {
        Id = Guid.NewGuid();
        SongId = songId;
        LyricsSegmentData = lyricsSegmentData;
    }
}
