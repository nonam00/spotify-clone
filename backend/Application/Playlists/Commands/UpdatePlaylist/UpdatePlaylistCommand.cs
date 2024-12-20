using MediatR;

namespace Application.Playlists.Commands.UpdatePlaylist
{
    public class UpdatePlaylistCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public string? ImagePath { get; init; } 
    }
}
