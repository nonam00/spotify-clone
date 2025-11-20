using FluentValidation;

namespace Application.Moderators.Queries.GetModeratorInfo;

public class GetModeratorInfoQueryValidator : AbstractValidator<GetModeratorInfoQuery>    
{
    public GetModeratorInfoQueryValidator()
    {
        RuleFor(command => command.ModeratorId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");   
    }
}