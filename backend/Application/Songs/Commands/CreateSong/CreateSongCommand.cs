using MediatR;

namespace Application.Songs.Commands.CreateSong
{
    public class CreateSongCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string SongPath { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
    }
}
