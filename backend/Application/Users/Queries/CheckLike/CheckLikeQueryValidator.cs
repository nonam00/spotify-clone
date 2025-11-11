using FluentValidation;

namespace Application.Users.Queries.CheckLike;

public class CheckLikeQueryValidator : AbstractValidator<CheckLikeQuery>
{
    public CheckLikeQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");
        
        RuleFor(q => q.SongId)
            .NotEqual(Guid.Empty)
            .WithMessage("Song ID is required")
            .WithErrorCode("400");;
    }
}