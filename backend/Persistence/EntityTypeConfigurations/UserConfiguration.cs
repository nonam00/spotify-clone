using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain;

namespace Persistence.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
            
        builder.HasIndex(u => u.Email).IsUnique();
            
        builder.HasMany(u => u.Playlists)
            .WithOne()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rf => rf.User)
            .HasForeignKey(rf => rf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(false);
    }
}