using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Models;
using Domain.ValueObjects;

namespace Persistence.EntityTypeConfigurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasIndex(s => s.Title)
            .IsUnique(false)
            .HasMethod("gin");

        builder.Property(s => s.Author)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasIndex(s => s.Author)
            .IsUnique(false)
            .HasMethod("gin");
        
        builder.Property(s => s.SongPath)
            .HasConversion(
                path => path.Value,
                value => new FilePath(value))
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(s => s.ImagePath)
            .HasConversion(
                path => path.Value,
                value => new FilePath(value))
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(s => s.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(song => song.CreatedAt);
            
        builder.HasOne(song => song.Uploader)
            .WithMany()
            .HasForeignKey(song => song.UploaderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}