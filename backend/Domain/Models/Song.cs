using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Models;

// Song Aggregate Root
public class Song : AggregateRoot<Guid>
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public FilePath SongPath { get; private set; } = null!;
    public FilePath ImagePath { get; private set; } = null!;
    public Guid? UploaderId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties for EF Core (not part of domain logic)
    public virtual User? Uploader { get; private set; }

    private Song() { } // For EF Core
    
    public static Song Create(string title, FilePath songPath, FilePath imagePath, string author, Guid? uploaderId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));

        var song = new Song
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Title = title.Trim(),
            SongPath = songPath,
            ImagePath = imagePath,
            Author = author.Trim(),
            UploaderId = uploaderId
        };

        return song;
    }
}