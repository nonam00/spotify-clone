using FluentValidation;

namespace Application.LikedSongs.Queries.CheckLikedSong;

public class CheckLikedSongQueryValidator : AbstractValidator<CheckLikedSongQuery>
{
    public CheckLikedSongQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(q => q.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");;
    }
}