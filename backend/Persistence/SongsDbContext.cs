using Domain;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityTypeConfigurations;

namespace Persistence
{
    public class SongsDbContext : DbContext, ISongsDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<LikedSong> LikedSongs { get; set; }
        public SongsDbContext(DbContextOptions<SongsDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SongConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new LikedSongConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
