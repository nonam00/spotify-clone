using FluentValidation;

namespace Application.Users.Commands.UnlikeSong;

public class UnlikeSongCommandValidator : AbstractValidator<UnlikeSongCommand>
{
    public UnlikeSongCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");
    }
}