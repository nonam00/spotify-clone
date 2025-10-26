using FluentValidation;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandValidator : AbstractValidator<DeleteRefreshTokenCommand>
{
    public DeleteRefreshTokenCommandValidator()
    {
        RuleFor(q => q.RefreshToken)
            .NotEqual(string.Empty)
            .WithMessage("Refresh token is required")
            .WithErrorCode("400");
    }
}