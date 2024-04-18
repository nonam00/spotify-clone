using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListByTitle
{
    public class GetSongListByTitleQueryValidator
        : AbstractValidator<GetSongListByTitleQuery>
    {
        public GetSongListByTitleQueryValidator()
        {
            RuleFor(query => query.SearchString.Trim()).NotEqual(String.Empty); 
        }
    }
}
