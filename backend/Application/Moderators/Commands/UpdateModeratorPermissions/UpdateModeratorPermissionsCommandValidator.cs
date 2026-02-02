using FluentValidation;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public class UpdateModeratorPermissionsCommandValidator : AbstractValidator<UpdateModeratorPermissionsCommand>
{
    public UpdateModeratorPermissionsCommandValidator()
    {
        RuleFor(command => command.ManagingModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Managing moderator ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ModeratorToUpdatePermissionsId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
        
        RuleFor(command => command.ModeratorToUpdatePermissionsId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator to update ID is required")
            .WithErrorCode("400")
            .NotEqual(c => c.ManagingModeratorId)
            .WithMessage("Moderator cannot manage himself")
            .WithErrorCode("400");
    }
}

