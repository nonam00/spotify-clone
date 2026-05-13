namespace Domain.ValueObjects;

// Time code measures in fractions of second 
public sealed record LyricsSegmentData
{
    public double Start { get; }
    public double End { get; }
    public string Text { get; }
    public int Order { get; }
    
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