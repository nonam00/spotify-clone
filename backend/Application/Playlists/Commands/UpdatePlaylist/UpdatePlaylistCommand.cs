using MediatR;

namespace Application.Playlists.Commands.UpdatePlaylist
{
    public class UpdatePlaylistCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; } 
    }
}
