namespace Domain;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? BillingAddress { get; set; }
    public string? PaymentMethod { get; set; }
    public IList<LikedSong> LikedSongs { get; } = new List<LikedSong>();
    public IList<Playlist> Playlists { get; } = new List<Playlist>();
}