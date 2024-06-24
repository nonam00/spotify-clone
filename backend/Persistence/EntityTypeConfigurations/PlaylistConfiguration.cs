using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.HasMany(p => p.Songs)
                   .WithMany()
                   .UsingEntity<PlaylistSong>();

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
