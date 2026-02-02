using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Models;

// Song Aggregate Root
public class Song : AggregateRoot<Guid>
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public FilePath SongPath { get; private init; } = null!;
    public FilePath ImagePath { get; private init; } = null!;
    public Guid? UploaderId { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool MarkedForDeletion { get; private set; }

    // Navigation properties for EF Core (not part of domain logic)
    public virtual User? Uploader { get; private set; }

    private Song() { } // For EF Core
    
    // Cannot make internal because of the tests
    public static Song Create(string title, string author, FilePath songPath, FilePath imagePath,
        Guid? uploaderId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Author cannot be empty", nameof(author));
        }

        var song = new Song
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Author = author.Trim(),
            SongPath = songPath,
            ImagePath = imagePath,
            UploaderId = uploaderId,
            IsPublished = false,
            MarkedForDeletion = false,
            CreatedAt = DateTime.UtcNow,
        };

        return song;
    }
    
    // TODO: make internal after rewriting tests
    public Result Publish()
    {
        if (MarkedForDeletion)
        {
            return Result.Failure(SongDomainErrors.CannotPublishMarkedForDeletion);
        }
        
        if (IsPublished)
        {
            return Result.Failure(SongDomainErrors.AlreadyPublished);
        }
        
        IsPublished = true;
        return Result.Success();
    }

    internal Result Unpublish()
    {
        if (!IsPublished)
        {
            return Result.Failure(SongDomainErrors.AlreadyUnpublished);
        }
        IsPublished = false;
        return Result.Success();
    }

    internal Result MarkForDeletion()
    {
        if (IsPublished)
        {
            return Result.Failure(SongDomainErrors.CannotDeletePublished);
        }

        if (MarkedForDeletion)
        {
            return Result.Failure(SongDomainErrors.AlreadyMarkedForDeletion);
        }
        
        MarkedForDeletion = true;
        return Result.Success();
    } 
}

public static class SongDomainErrors
{
    public static readonly Error AlreadyPublished =
        new(nameof(AlreadyPublished), "The song is already published.");
    
    public static readonly Error AlreadyUnpublished =
        new(nameof(AlreadyUnpublished), "The song is already unpublished.");
    
    public static readonly Error CannotDeletePublished =
        new(nameof(CannotDeletePublished), "Published song cannot be deleted.");
    
    public static readonly Error CannotPublishMarkedForDeletion = new(
        nameof(CannotPublishMarkedForDeletion),
        "Song that was marked for deletion cannot be published.");
    
    public static readonly Error AlreadyMarkedForDeletion =
        new(nameof(AlreadyMarkedForDeletion), "The song is already marked for deletion.");
}