using Microsoft.EntityFrameworkCore;

using Domain;

namespace Persistence;

public class SongsDbContext(DbContextOptions<SongsDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;
    public DbSet<LikedSong> LikedSongs { get; set; } = null!;
    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<PlaylistSong> PlaylistSongs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(SongsDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}