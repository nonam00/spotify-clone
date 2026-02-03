using System.Security.Cryptography;
using Domain.Common;

namespace Domain.Models;

public class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime Expires { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsExpired;
    
    // Navigation property
    public virtual User User { get; private set; } = null!;

    private RefreshToken() { } // For EF Core

    internal static RefreshToken Create(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateRefreshTokenValue(),
            Expires = DateTime.UtcNow.AddDays(14),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    internal void UpdateToken()
    {
        Token = GenerateRefreshTokenValue();
        Expires = DateTime.UtcNow.AddDays(14);
    }
    
    private static string GenerateRefreshTokenValue() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
