using FluentValidation;

namespace Application.Moderators.Commands.DeleteSong;

public class DeleteSongCommandValidator : AbstractValidator<DeleteSongCommand>
{
    public DeleteSongCommandValidator()
    {
        RuleFor(c => c.ModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator ID is required")
            .WithErrorCode("400");
        
        RuleFor(c => c.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");
    }
}