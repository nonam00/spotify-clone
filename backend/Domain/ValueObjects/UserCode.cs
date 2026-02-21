using System.Security.Cryptography;

namespace Domain.ValueObjects;

public class UserCode
{
    private const ushort CodeLength = 6;
    
    public readonly string Value = RandomNumberGenerator.GetString("0123456789", CodeLength);
    public readonly TimeSpan CodeExpiry = TimeSpan.FromMinutes(15);

    public static implicit operator string(UserCode code) => code.Value;
}