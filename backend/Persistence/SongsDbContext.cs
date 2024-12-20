using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Interfaces;
using Persistence.EntityTypeConfigurations;

namespace Persistence
{
    public class SongsDbContext(DbContextOptions<SongsDbContext> options) : DbContext(options), ISongsDbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<LikedSong> LikedSongs { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistSong> PlaylistSongs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SongConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new LikedSongConfiguration());
            builder.ApplyConfiguration(new PlaylistConfiguration());
            builder.ApplyConfiguration(new PlaylistSongConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
