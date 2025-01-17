﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
    public class LikedSongConfiguration: IEntityTypeConfiguration<LikedSong>
    {
        public void Configure(EntityTypeBuilder<LikedSong> builder)
        {
            builder.HasKey(song => new { song.UserId, song.SongId });

            builder.HasOne(liked => liked.Song)
                   .WithMany()
                   .HasForeignKey(liked => liked.SongId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(liked => liked.User)
                   .WithMany(user => user.LikedSongs)
                   .HasForeignKey(likedSong => likedSong.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(liked => liked.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
