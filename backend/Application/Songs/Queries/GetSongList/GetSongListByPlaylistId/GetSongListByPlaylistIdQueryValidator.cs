using FluentValidation;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId;

public class GetSongListByPlaylistIdQueryValidator : AbstractValidator<GetSongListByPlaylistIdQuery>
{
    public GetSongListByPlaylistIdQueryValidator()
    {
        RuleFor(command => command.PlaylistId)
            .NotEqual(Guid.Empty)
            .WithMessage("Playlist ID is required")
            .WithErrorCode("400");
        
    }
}