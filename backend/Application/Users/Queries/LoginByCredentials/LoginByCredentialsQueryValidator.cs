using FluentValidation;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQueryValidator : AbstractValidator<LoginByCredentialsQuery>
{
    public LoginByCredentialsQueryValidator()
    {
        RuleFor(q => q.Email).EmailAddress().WithMessage("Wrong email format");
        RuleFor(q => q.Password).NotEqual(string.Empty);
    }
}