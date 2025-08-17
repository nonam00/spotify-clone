using FluentValidation;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword)
            .MinimumLength(8)
            .WithMessage("Password length must be equal or greater than 8")
            .WithErrorCode("400");
        
        RuleFor(command => command.NewPassword)
            .MinimumLength(8)
            .WithMessage("Password length must be equal or greater than 8")
            .WithErrorCode("400");
    }
}