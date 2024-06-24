using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId
{
    public class GetLikedSongListBySearchStringAndPlaylistIdQueryValidator
        : AbstractValidator<GetLikedSongListBySearchStringAndPlaylistIdQuery>
    {
        public GetLikedSongListBySearchStringAndPlaylistIdQueryValidator()
        {
            RuleFor(q => q.UserId).NotEqual(Guid.Empty);
            RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
            RuleFor(q => q.SearchString).NotEqual(string.Empty);
        }
    }
}
