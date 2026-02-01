using FluentValidation;

namespace Application.Users.Commands.CleanUserRefreshTokens;

public class CleanUserRefreshTokensCommandValidator : AbstractValidator<CleanUserRefreshTokensCommand>
{
    public CleanUserRefreshTokensCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
    }
}