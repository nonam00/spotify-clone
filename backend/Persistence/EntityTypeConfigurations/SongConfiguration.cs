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

        builder.Property(s => s.Author)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property<string>("TitleLower")
            .HasComputedColumnSql("lower(f_unaccent(trim(\"title\")))", stored: true);
            
        builder.Property<string>("AuthorLower")
            .HasComputedColumnSql("lower(f_unaccent(trim(\"author\")))", stored: true);
        
        // Gin indexes for trigram searching
        builder.HasIndex("TitleLower")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
            
        builder.HasIndex("AuthorLower")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
        
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

        builder.Property(s => s.IsPublished);
        
        builder.Property(song => song.CreatedAt);
            
        builder.HasOne(song => song.Uploader)
            .WithMany(u => u.PublishedSongs)
            .HasForeignKey(song => song.UploaderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}