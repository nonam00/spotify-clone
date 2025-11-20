using FluentValidation;

namespace Application.Songs.Commands.UnpublishSong;

public class UnpublishSongCommandValidator : AbstractValidator<UnpublishSongCommand>
{
    public UnpublishSongCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
    }
}