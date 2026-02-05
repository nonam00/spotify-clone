using Domain.Common;
using Domain.Errors;
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
    internal static Result<Song> Create(
        string title, string author, FilePath songPath, FilePath imagePath, Guid? uploaderId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Song>.Failure(SongDomainErrors.TitleCannotBeEmpty);
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            return Result<Song>.Failure(SongDomainErrors.AuthorCannotBeEmpty);
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

        return Result<Song>.Success(song);
    }
    
    internal Result Publish()
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

    internal Result CancelMarkForDeletion()
    {
        if (!MarkedForDeletion)
        {
            return Result.Failure(SongDomainErrors.NotMarkedForDeletion);
        }
        
        MarkedForDeletion = false;
        return Result.Success();
    }
}