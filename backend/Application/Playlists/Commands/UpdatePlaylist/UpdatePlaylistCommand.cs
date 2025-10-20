using Application.Shared.Messaging;

namespace Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommand : ICommand<string?>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? ImagePath { get; init; } 
}