using FluentValidation;

namespace Application.Users.Commands.SendRestoreToken;

public class SendRestoreTokenCommandValidator : AbstractValidator<SendRestoreTokenCommand>
{
    public SendRestoreTokenCommandValidator()
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
    }
}