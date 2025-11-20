using FluentValidation;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public class UpdateModeratorPermissionsCommandValidator : AbstractValidator<UpdateModeratorPermissionsCommand>
{
    public UpdateModeratorPermissionsCommandValidator()
    {
        RuleFor(c => c.ModeratorId).NotEmpty();
    }
}

