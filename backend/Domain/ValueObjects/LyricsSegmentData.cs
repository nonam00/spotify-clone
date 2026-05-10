namespace Domain.ValueObjects;

// Time code measures in fractions of second 
public sealed record LyricsSegmentData
{
    public double Start { get; private init; }
    public double End { get; private init; }
    public string Text { get; private init; }
    public int Order { get; private init; }
    
    public LyricsSegmentData(double start, double end, string text, int order)
    {
        if (start > end)
        {
            throw new Exception("Lyrics segment start must be earlier than end");
        }
        if (order < 1)
        {
            throw new Exception("Lyrics segment order must be at least 1");
        }
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new Exception("Lyrics segment text cannot be empty");
        }
        
        Start = start;
        End = end;
        Text = text;
        Order = order;
    }
}