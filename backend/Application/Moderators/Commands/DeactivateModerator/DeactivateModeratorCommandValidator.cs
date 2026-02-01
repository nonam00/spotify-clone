using FluentValidation;

namespace Application.Moderators.Commands.DeactivateModerator;

public class DeactivateModeratorCommandValidator : AbstractValidator<DeactivateModeratorCommand>
{
    public DeactivateModeratorCommandValidator()
    {
        RuleFor(command => command.ManagingModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Managing moderator ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ModeratorToDeactivateId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
        
        RuleFor(command => command.ModeratorToDeactivateId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator to deactivate ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ManagingModeratorId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
    }
}

