using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
  public class PlaylistSongConfiguration : IEntityTypeConfiguration<PlaylistSong>
  {
    public void Configure(EntityTypeBuilder<PlaylistSong> builder)
    {
      builder.HasKey(ps => new { ps.PlaylistId, ps.SongId });

      builder.Property(ps => ps.CreatedAt)
             .HasDefaultValueSql("CURRENT_DATE");
    }
  }
}
