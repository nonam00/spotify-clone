using MediatR;

namespace Application.Songs.Commands.CreateSong
{
    public class CreateSongCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string SongPath { get; set; }
        public string ImagePath { get; set; }
    }
}
