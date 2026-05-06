namespace Domain.ValueObjects;

public class LyricsFragment
{
    public Guid Id { private get; init; }
    public Guid SongId { private get; init; }
    public double Start { private get; init; }
    public double End { private get; init; }
    public string Text { private get; init; } = null!;
    public int Order { private get; init; }
}