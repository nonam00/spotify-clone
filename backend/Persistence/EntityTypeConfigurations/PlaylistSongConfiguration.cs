using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations;

public class PlaylistSongConfiguration : IEntityTypeConfiguration<PlaylistSong>
{
    public void Configure(EntityTypeBuilder<PlaylistSong> builder)
    {
        builder.HasKey(ps => new { ps.PlaylistId, ps.SongId });

        builder.HasOne(ps => ps.Playlist)
            .WithMany()
            .HasForeignKey(ps => ps.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Song)
            .WithMany()
            .HasForeignKey(ps => ps.SongId)
            .OnDelete(DeleteBehavior.Cascade);
    
        builder.Property(ps => ps.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    } 
}