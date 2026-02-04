using FluentValidation;

namespace Application.Users.Commands.RestoreUserAccess;

public class RestoreUserAccessCommandValidator : AbstractValidator<RestoreUserAccessCommand>
{
    public RestoreUserAccessCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("400")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("400")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters")
            .WithErrorCode("400");

        RuleFor(command => command.RestoreCode)
            .NotEmpty()
            .WithMessage("Restore code is required")
            .WithErrorCode("400")
            .MinimumLength(6)
            .WithMessage("Restore code must be at least 6 characters long")
            .WithErrorCode("400")
            .MaximumLength(10)
            .WithMessage("Restore code cannot exceed 10 characters")
            .WithErrorCode("400");
    }
}