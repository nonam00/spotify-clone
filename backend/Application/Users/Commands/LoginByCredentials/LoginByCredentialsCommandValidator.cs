using FluentValidation;

namespace Application.Users.Commands.LoginByCredentials;

public class LoginByCredentialsCommandValidator : AbstractValidator<LoginByCredentialsCommand>
{
    public LoginByCredentialsCommandValidator()
    {
        RuleFor(query => query.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("400")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters")
            .WithErrorCode("400");

        RuleFor(query => query.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .WithErrorCode("400")
            .MinimumLength(1)
            .WithMessage("Password cannot be empty")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters")
            .WithErrorCode("400");
    }
}