using FluentValidation;

namespace Application.Moderators.Commands.ActivateUser;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(c => c.ModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator ID is required.")
            .WithErrorCode("400");
        
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserID is required.")
            .WithErrorCode("400");
    }
}