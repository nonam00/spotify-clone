namespace Domain
{
    public class Song
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string SongPath { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public IList<LikedSong> Liked { get; } = new List<LikedSong>();

    }
}
