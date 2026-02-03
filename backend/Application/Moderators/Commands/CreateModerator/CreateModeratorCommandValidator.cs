using FluentValidation;

namespace Application.Moderators.Commands.CreateModerator;

public class CreateModeratorCommandValidator : AbstractValidator<CreateModeratorCommand>
{
    public CreateModeratorCommandValidator()
    {
        RuleFor(command => command.ManagingModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("Managing moderator ID is required")
            .WithErrorCode("400");
        
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("400")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("400")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters")
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
            .NotEmpty()
            .WithMessage("Full name is required")
            .WithErrorCode("400")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters")
            .WithErrorCode("400");
    }
}