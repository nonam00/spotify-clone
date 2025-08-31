using Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rf => rf.Id);
        
        builder.Property(rf => rf.Token).HasMaxLength(200);
        
        builder.HasIndex(rf => rf.Token).IsUnique();
    }
}