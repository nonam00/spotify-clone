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
        builder.Property(m => m.Id).ValueGeneratedNever();
        
        // Initial moderator
        builder.HasData(new
        {
            Id = Guid.Parse("12DBC10A-A7A9-47A3-9A1B-513AAE383F1F"),
            Email = new Email("admin@admin.com"),
            FullName = "Admin",
            PasswordHash = new PasswordHash("$2a$11$1Aw9/S0I8DnN7VDPmP4EuOl3qREfsNBFwfc8JOIqhldNYDYMBARLC"),
            IsActive = true,
            CreatedAt = DateTime.Parse("2023-04-15").ToUniversalTime()
        });
        
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

            // Initial moderator permissions
            permissionsBuilder.HasData(new
            {
                ModeratorId = Guid.Parse("12DBC10A-A7A9-47A3-9A1B-513AAE383F1F"),
                CanManageUsers = true,
                CanManageContent = true,
                CanViewReports = true,
                CanManageModerators = true
            });
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