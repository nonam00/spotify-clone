using FluentValidation;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public class UpdateModeratorPermissionsCommandValidator : AbstractValidator<UpdateModeratorPermissionsCommand>
{
    public UpdateModeratorPermissionsCommandValidator()
    {
        RuleFor(command => command.ManagingModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Managing moderator ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.ModeratorToUpdateId)
            .NotEqual(Guid.Empty)
            .WithMessage("Moderator to update ID is required")
            .WithErrorCode("400");
    }
}

