using FluentValidation;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQueryValidator : AbstractValidator<LoginByRefreshTokenQuery>
{
    public LoginByRefreshTokenQueryValidator()
    {
        RuleFor(q => q.RefreshToken).NotEqual(string.Empty);
    }
}