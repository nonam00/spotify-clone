using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListByAny
{
    public class GetSongListByAnyQueryValidator : AbstractValidator<GetSongListByAnyQuery>
    {
        public GetSongListByAnyQueryValidator()
        {
            RuleFor(query => query.SearchString.Trim()).NotEqual(string.Empty);
        }
    }
}
