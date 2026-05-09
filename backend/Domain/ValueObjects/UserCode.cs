using System.Security.Cryptography;

namespace Domain.ValueObjects;

public sealed record UserCode
{
    private const ushort CodeLength = 6;

    private readonly string _value = RandomNumberGenerator.GetString("0123456789", CodeLength);
    public readonly TimeSpan CodeExpiry = TimeSpan.FromMinutes(15);

    public static implicit operator string(UserCode code) => code._value;
}