using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID cannot be empty")
            .WithErrorCode("400");

        RuleFor(command => command.FullName)
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters")
            .WithErrorCode("400")
            .When(command => !string.IsNullOrEmpty(command.FullName));

        RuleFor(command => command.AvatarPath)
            .MaximumLength(500)
            .WithMessage("Avatar path cannot exceed 500 characters")
            .WithErrorCode("400")
            .When(command => !string.IsNullOrEmpty(command.AvatarPath));
    }
}
