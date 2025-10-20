using FluentValidation;

namespace Application.LikedSongs.Commands.CreateLikedSong;

public class CreateLikedSongCommandValidator : AbstractValidator<CreateLikedSongCommand>
{
    public CreateLikedSongCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");;
    }
}