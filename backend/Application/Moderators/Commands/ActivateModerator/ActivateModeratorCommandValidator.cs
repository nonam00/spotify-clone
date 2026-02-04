using FluentValidation;

namespace Application.Moderators.Commands.ActivateModerator;

public class ActivateModeratorCommandValidator : AbstractValidator<ActivateModeratorCommand>
{
    public ActivateModeratorCommandValidator()
    {
        RuleFor(command => command.ManagingModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Managing moderator ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ModeratorToActivateId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
        
        RuleFor(command => command.ModeratorToActivateId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator to activate ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ManagingModeratorId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
    }
}

