using FluentValidation;

namespace Application.Moderators.Queries.LoginByCredentials;

public class LoginByCredentialsQueryValidator : AbstractValidator<LoginByCredentialsQuery>
{
    public LoginByCredentialsQueryValidator()
    {
        RuleFor(query => query.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("400")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("400")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters")
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