using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Models;

// Moderator Aggregate Root
public class Moderator : AggregateRoot<Guid>
{
    public Email Email { get; private init; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string? FullName { get; private init; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ModeratorPermissions Permissions { get; private set; } = null!;

    private Moderator() { }

    public static Moderator Create(Email email, PasswordHash passwordHash, string? fullName = null, ModeratorPermissions? permissions = null)
    {
        var moderator = new Moderator
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName?.Trim(),
            IsActive = true,
            Permissions = permissions ?? ModeratorPermissions.CreateDefault(),
            CreatedAt = DateTime.UtcNow
        };

        return moderator;
    }

    public void ChangePassword(PasswordHash newPasswordHash) => PasswordHash = newPasswordHash;

    public void Deactivate() => IsActive = false;
    
    public void Activate() => IsActive = true;

    public void UpdatePermissions(ModeratorPermissions permissions) => Permissions = permissions;
}