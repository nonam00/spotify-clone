using FluentValidation;

namespace Application.Users.Commands.LikeSong;

public class LikeSongCommandValidator : AbstractValidator<LikeSongCommand>
{
    public LikeSongCommandValidator()
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