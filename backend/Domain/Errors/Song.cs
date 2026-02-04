using Domain.Common;

namespace Domain.Errors;

public static class SongDomainErrors
{
    public static readonly Error TitleCannotBeEmpty =
        new(nameof(TitleCannotBeEmpty), "Title cannot be empty.");
    
    public static readonly Error AuthorCannotBeEmpty =
        new(nameof(AuthorCannotBeEmpty), "Author cannot be empty.");
    
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