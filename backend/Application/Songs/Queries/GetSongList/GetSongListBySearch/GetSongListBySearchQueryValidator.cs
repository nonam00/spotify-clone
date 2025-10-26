using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListBySearch;

public class GetSongListBySearchQueryValidator : AbstractValidator<GetSongListBySearchQuery>
{
    public GetSongListBySearchQueryValidator()
    {
        RuleFor(query => query.SearchString.Trim())
            .NotEqual(string.Empty)
            .WithMessage("Search string is required")
            .WithErrorCode("400");;
    }    
}