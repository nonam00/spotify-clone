using FluentValidation;

namespace Application.Songs.Queries.GetSongListBySearch;

public class GetSongListBySearchQueryValidator : AbstractValidator<GetSongListBySearchQuery>
{
    public GetSongListBySearchQueryValidator()
    {
        RuleFor(query => query.SearchString.Trim())
            .NotEqual(string.Empty)
            .WithMessage("Search string is required")
            .WithErrorCode("400")
            .MinimumLength(3)
            .WithMessage("Search string must be at least 3 characters long")
            .WithErrorCode("400")
            .MaximumLength(200)
            .WithMessage("Search string length must be less than 200 characters")
            .WithErrorCode("400");
        
        RuleFor(query => query.SearchCriteria)
            .NotNull()
            .WithMessage("Search criteria is required")
            .WithErrorCode("400");
    }    
}