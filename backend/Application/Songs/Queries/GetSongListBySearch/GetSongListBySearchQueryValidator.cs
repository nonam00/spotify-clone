using FluentValidation;

namespace Application.Songs.Queries.GetSongListBySearch;

public class GetSongListBySearchQueryValidator : AbstractValidator<GetSongListBySearchQuery>
{
    public GetSongListBySearchQueryValidator()
    {
        RuleFor(query => query.SearchString.Trim())
            .NotEqual(string.Empty)
            .WithMessage("Search string is required")
            .WithErrorCode("400");
        
        RuleFor(query => query.SearchCriteria)
            .NotNull()
            .WithMessage("Search criteria is required")
            .WithErrorCode("400");
    }    
}