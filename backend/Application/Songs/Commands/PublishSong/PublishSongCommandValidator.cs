using FluentValidation;

namespace Application.Songs.Commands.PublishSong;

public class PublishSongCommandValidator : AbstractValidator<PublishSongCommand>
{
    public PublishSongCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
    }
}