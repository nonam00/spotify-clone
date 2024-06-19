using MediatR;

namespace Application.Playlists.Commands.UpdatePlaylist
{
    public class UpdatePlaylistCommand : IRequest
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; } 
    }
}
