using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Models;

namespace Persistence.EntityTypeConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).ValueGeneratedNever();

        builder.Property(rt => rt.UserId).IsRequired();
        
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
        builder.HasIndex(rt => rt.Token).IsUnique();

        builder.Property(rt => rt.CreatedAt);
    }
}