namespace Domain
{
    public class PlaylistSong
    {
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public Guid SongId { get; set; }
        public Song Song { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
