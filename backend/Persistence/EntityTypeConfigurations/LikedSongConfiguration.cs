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

            builder.HasOne(liked => liked.Song)
                   .WithMany()
                   .HasForeignKey(liked => liked.SongId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(song => song.User)
                   .WithMany(user => user.LikedSongs)
                   .HasForeignKey(likedSong => likedSong.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        
            builder.Property(song => song.CreatedAt)
                   .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
