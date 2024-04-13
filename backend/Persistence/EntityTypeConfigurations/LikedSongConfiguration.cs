using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
    public class LikedSongConfiguration: IEntityTypeConfiguration<LikedSong>
    {
        public void Configure(EntityTypeBuilder<LikedSong> builder)
        {
            builder.HasKey(song => new { song.UserId, song.SongId });
            builder.HasIndex(song => new { song.UserId, song.SongId }).IsUnique();

            // TODO: configure foreign relations, cascade update and delete relations
        }
    }
}
