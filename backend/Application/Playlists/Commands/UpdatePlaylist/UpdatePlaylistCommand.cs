using MediatR;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommand : IRequest<string?>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? ImagePath { get; init; } 
}