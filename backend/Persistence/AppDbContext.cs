using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Domain.Models;

namespace Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;
    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    internal DbSet<LikedSong> LikedSongs => Set<LikedSong>();
    internal DbSet<PlaylistSong> PlaylistSongs => Set<PlaylistSong>();
    public DbSet<Moderator> Moderators { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Ignore<DomainEvent>();
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}