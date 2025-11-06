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
        
        RuleFor(q => q.SearchString)
            .NotEqual(string.Empty)
            .WithMessage("Search string is required")
            .WithErrorCode("400");;
    }
}