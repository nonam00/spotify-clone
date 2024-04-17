using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
    public class LikedSongConfiguration: IEntityTypeConfiguration<LikedSong>
    {
        public void Configure(EntityTypeBuilder<LikedSong> builder)
        {
            // Primary key
            builder.HasKey(song => new { song.UserId, song.SongId });
            builder.HasIndex(song => new { song.UserId, song.SongId }).IsUnique();

            // Configuration of foreign relations, cascade update and delete relations

            // Song
            builder.HasOne(liked => liked.Song)
                .WithMany(song => song.Liked)
                .HasForeignKey(liked => liked.SongId)
                .HasPrincipalKey(song =>  song.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // User
            builder.HasOne(song => song.User)
                .WithMany(user => user.LikedSongs)
                .HasForeignKey(likedSong => likedSong.UserId)
                .HasPrincipalKey(likedSong => likedSong.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
