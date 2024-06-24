using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId
{
    public class GetSongListByPlaylistIdQueryValidator
      : AbstractValidator<GetSongListByPlaylistIdQuery>
    {
        public GetSongListByPlaylistIdQueryValidator()
        {
            RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
        }
    }
}
