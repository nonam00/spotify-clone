using FluentValidation;

namespace Application.Moderators.Commands.DeactivateUser;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(c => c.ModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator ID is required.")
            .WithErrorCode("400");
        
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required.")
            .WithErrorCode("400");
    }
}