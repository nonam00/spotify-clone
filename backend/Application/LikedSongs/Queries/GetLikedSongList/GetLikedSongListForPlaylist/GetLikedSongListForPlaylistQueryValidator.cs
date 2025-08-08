using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQueryValidator : AbstractValidator<GetLikedSongListForPlaylistQuery>
{
    public GetLikedSongListForPlaylistQueryValidator()
    {
        RuleFor(q => q.PlaylistId).NotEqual(Guid.Empty);
        RuleFor(q => q.UserId).NotEqual(Guid.Empty);
    }
}