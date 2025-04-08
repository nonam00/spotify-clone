using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Domain;
using Application.Interfaces;

namespace Persistence
{
    public class SongsDbContext(DbContextOptions<SongsDbContext> options)
        : DbContext(options), ISongsDbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<LikedSong> LikedSongs { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistSong> PlaylistSongs { get; set; } = null!;

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(SongsDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
