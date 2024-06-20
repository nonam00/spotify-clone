namespace Domain
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
