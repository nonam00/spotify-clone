using FluentValidation;

namespace Application.Users.Commands.ActivateUserByConfirmationCode;

public class ActivateUserByConfirmationCodeCommandValidator : AbstractValidator<ActivateUserByConfirmationCodeCommand>
{
    public ActivateUserByConfirmationCodeCommandValidator()
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

        RuleFor(command => command.ConfirmationCode)
            .NotEmpty()
            .WithMessage("Confirmation code is required")
            .WithErrorCode("400")
            .MinimumLength(6)
            .WithMessage("Confirmation code must be at least 6 characters long")
            .WithErrorCode("400")
            .MaximumLength(10)
            .WithMessage("Confirmation code cannot exceed 10 characters")
            .WithErrorCode("400");
    }
}