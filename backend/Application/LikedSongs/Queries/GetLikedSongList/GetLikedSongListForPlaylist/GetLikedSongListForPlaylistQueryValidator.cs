using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

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