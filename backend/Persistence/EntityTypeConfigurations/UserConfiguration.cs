using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Models;
using Domain.ValueObjects;

namespace Persistence.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => new PasswordHash(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FullName)
            .HasMaxLength(255)
            .IsRequired(false);
        
        builder.Property(u => u.AvatarPath)
            .HasConversion(
                path => path.Value,
                value => new FilePath(value))
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasMany(u => u.PublishedSongs)
            .WithOne(s => s.Uploader)
            .HasForeignKey(u => u.UploaderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.Playlists)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}