using FluentValidation;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEqual(string.Empty)
            .EmailAddress();

        RuleFor(c => c.ConfirmationCode)
            .NotEqual(string.Empty)
            .MinimumLength(6);
    }
}