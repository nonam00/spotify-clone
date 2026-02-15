using FluentValidation;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("400")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("400")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters")
            .WithErrorCode("400");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .WithErrorCode("400")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters")
            .WithErrorCode("400");
        
        RuleFor(command => command.FullName)
            .Must(name => name is not null &&  name.Length <= 255)
            .WithMessage("User name cannot exceed 255 characters")
            .WithErrorCode("400")
            .When(command => command.FullName is not null);
    }
}