namespace Domain;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public string? AvatarPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
    public IList<LikedSong> LikedSongs { get; } = new List<LikedSong>();
    public IList<Playlist> Playlists { get; } = new List<Playlist>();
}