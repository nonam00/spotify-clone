namespace Domain
{
    public class LikedSong
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid SongId { get; set; }
        public Song Song { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
