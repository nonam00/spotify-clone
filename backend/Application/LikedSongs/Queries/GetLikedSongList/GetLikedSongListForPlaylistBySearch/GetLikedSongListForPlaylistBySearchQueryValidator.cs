using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQueryValidator 
    : AbstractValidator<GetLikedSongListForPlaylistBySearchQuery>
{
    public GetLikedSongListForPlaylistBySearchQueryValidator()
    {
        RuleFor(q => q.UserId).NotEqual(Guid.Empty);
        RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
        RuleFor(q => q.SearchString).NotEqual(string.Empty);
    }
}