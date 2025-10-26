using FluentValidation;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required")
            .WithErrorCode("400")
            .MinimumLength(8)
            .WithMessage("Current password must be at least 8 characters long")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("Current password cannot exceed 100 characters")
            .WithErrorCode("400");
        
        RuleFor(command => command.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .WithErrorCode("400")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("New password cannot exceed 100 characters")
            .WithErrorCode("400");

        RuleFor(command => command)
            .Must(command => command.CurrentPassword != command.NewPassword)
            .WithMessage("New password must be different from current password")
            .WithErrorCode("400");
    }
}