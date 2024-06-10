using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListByAuthor
{
    public class GetSongListByAuthorQueryValidator : AbstractValidator<GetSongListByAuthorQuery>
    {
        public GetSongListByAuthorQueryValidator()
        {
            RuleFor(query => query.SearchString.Trim()).NotEqual(String.Empty);
        }
    }
}
