using FluentValidation;

namespace Application.Moderators.Commands.UpdateModeratorStatus;

public class UpdateModeratorStatusCommandValidator : AbstractValidator<UpdateModeratorStatusCommand>
{
    public UpdateModeratorStatusCommandValidator()
    {
        RuleFor(c => c.ModeratorId).NotEmpty();
    }
}

