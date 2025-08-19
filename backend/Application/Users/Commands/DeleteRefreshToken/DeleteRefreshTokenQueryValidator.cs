using FluentValidation;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenQueryValidator : AbstractValidator<DeleteRefreshTokenQuery>
{
    public DeleteRefreshTokenQueryValidator()
    {
        RuleFor(q => q.RefreshToken).NotEqual(string.Empty);
    }
}