using FluentValidation;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommandValidator : AbstractValidator<DeleteLikedSongCommand>
{
    public DeleteLikedSongCommandValidator()
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