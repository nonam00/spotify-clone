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
            .Must(fullName => fullName is not null && fullName.Trim().Length <= 100)
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters")
            .WithErrorCode("400")
            .When(command => command.FullName is not null)
            .Must(fullName => fullName is not null && !string.IsNullOrWhiteSpace(fullName))
            .WithMessage("Full name cannot be empty if provided")
            .WithErrorCode("400")
            .When(command => command.FullName is not null);

        RuleFor(command => command.AvatarPath)
            .MaximumLength(500)
            .WithMessage("Avatar path cannot exceed 500 characters")
            .WithErrorCode("400")
            .When(command => !string.IsNullOrWhiteSpace(command.AvatarPath));
    }
}
