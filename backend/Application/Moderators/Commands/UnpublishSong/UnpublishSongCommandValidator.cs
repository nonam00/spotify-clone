using FluentValidation;

namespace Application.Moderators.Commands.UnpublishSong;

public class UnpublishSongCommandValidator : AbstractValidator<UnpublishSongCommand>
{
    public UnpublishSongCommandValidator()
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