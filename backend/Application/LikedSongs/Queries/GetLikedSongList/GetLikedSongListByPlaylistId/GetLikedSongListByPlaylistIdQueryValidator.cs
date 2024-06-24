using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListByPlaylistId
{
    public class GetLikedSongListByPlaylistIdQueryValidator
        : AbstractValidator<GetLikedSongListByPlaylistIdQuery>
    {
        public GetLikedSongListByPlaylistIdQueryValidator()
        {
            RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
        }
    }
}
