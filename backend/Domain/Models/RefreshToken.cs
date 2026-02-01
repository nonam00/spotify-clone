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

    public static RefreshToken Create(Guid userId, string token, DateTime expires)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty", nameof(token));
        }

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            Expires = expires,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    internal void UpdateToken(string newToken, DateTime newExpires)
    {
        Token = newToken;
        Expires = newExpires;
    }
}