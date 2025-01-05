using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations
{
    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.HasKey(song => song.Id);
            
            builder.HasIndex(song => song.Title)
                   .IsUnique(false)
                   .HasMethod("gin");

            builder.HasIndex(song => song.Author)
                   .IsUnique(false)
                   .HasMethod("gin");
            
            builder.HasOne(song => song.User)
                   .WithMany()
                   .HasForeignKey(song => song.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        
            builder.Property(song => song.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
