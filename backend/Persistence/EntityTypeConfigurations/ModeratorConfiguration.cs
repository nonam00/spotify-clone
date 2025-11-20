using Domain.Models;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class ModeratorConfiguration : IEntityTypeConfiguration<Moderator>
{
    public void Configure(EntityTypeBuilder<Moderator> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.OwnsOne(m => m.Permissions, permissionsBuilder =>
        {
            permissionsBuilder.Property(p => p.CanManageUsers)
                .HasColumnName("can_manage_users")
                .IsRequired();
                
            permissionsBuilder.Property(p => p.CanManageContent)
                .HasColumnName("can_manage_content")
                .IsRequired();
                
            permissionsBuilder.Property(p => p.CanViewReports)
                .HasColumnName("can_view_reports")
                .IsRequired();
                
            permissionsBuilder.Property(p => p.CanManageModerators)
                .HasColumnName("can_manage_moderators")
                .IsRequired();
        });
        
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasIndex(m => m.Email).IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => new PasswordHash(value))
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(m => m.FullName).HasMaxLength(255);
            
        builder.Property(m => m.IsActive).IsRequired();
        builder.HasIndex(m => m.IsActive);
            
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}