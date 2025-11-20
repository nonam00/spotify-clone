using FluentValidation;

namespace Application.Songs.Commands.DeleteSong;

public class DeleteSongCommandValidator : AbstractValidator<DeleteSongCommand>
{
    public DeleteSongCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
    }
}