using FluentValidation;

namespace Application.Songs.Queries.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQueryValidator : AbstractValidator<GetLikedSongListForPlaylistQuery>
{
    public GetLikedSongListForPlaylistQueryValidator()
    {
        RuleFor(q => q.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
    }
}