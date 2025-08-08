using FluentValidation;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoQuery>    
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(q => q.UserId).NotEqual(Guid.Empty);
    }
}