using FluentValidation;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQueryValidator : AbstractValidator<LoginByRefreshTokenQuery>
{
    public LoginByRefreshTokenQueryValidator()
    {
        RuleFor(query => query.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .WithErrorCode("400")
            .MinimumLength(10)
            .WithMessage("Refresh token must be at least 10 characters long")
            .WithErrorCode("400")
            .MaximumLength(500)
            .WithMessage("Refresh token cannot exceed 500 characters")
            .WithErrorCode("400");
    }
}