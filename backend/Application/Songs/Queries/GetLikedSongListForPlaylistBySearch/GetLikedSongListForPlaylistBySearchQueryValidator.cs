using FluentValidation;

namespace Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQueryValidator 
    : AbstractValidator<GetLikedSongListForPlaylistBySearchQuery>
{
    public GetLikedSongListForPlaylistBySearchQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");       
        
        RuleFor(q => q.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
        RuleFor(q => q.SearchString.Trim())
            .NotEqual(string.Empty)
            .WithMessage("Search string is required")
            .WithErrorCode("400")
            .MinimumLength(3)
            .WithMessage("Search string must be at least 3 characters long")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("Search string length must be less than 100 characters")
            .WithErrorCode("400");;
    }
}