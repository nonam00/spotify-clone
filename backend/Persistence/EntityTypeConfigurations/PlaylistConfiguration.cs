using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Models;
using Domain.ValueObjects;

namespace Persistence.EntityTypeConfigurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(p => p.Id);
        // To avoid problems when working with child collections 
        builder.Property(p => p.Id).ValueGeneratedNever();
        
        builder.Property(p => p.UserId).IsRequired();
        
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired(false);
        
        builder.Property(p => p.ImagePath)
            .HasConversion(
                path => path.Value,
                value => new FilePath(value))
            .HasMaxLength(255)
            .IsRequired(false);
        
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
    }
}