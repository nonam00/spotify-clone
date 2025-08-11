using FluentValidation;

namespace Application.Users.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(q => q.Email).EmailAddress().WithMessage("Wrong email format");
        RuleFor(q => q.Password).NotEqual(string.Empty);
    }
}