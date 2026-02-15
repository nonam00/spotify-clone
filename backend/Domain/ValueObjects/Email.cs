using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Email
{
    private const short MaxLength = 255;
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty", nameof(value));
        }

        if (value.Trim().Length > MaxLength)
        {
            throw new ArgumentException("Email cannot be longer than 255 characters", nameof(value));
        }

        if (!EmailRegex.IsMatch(value))
        {
            throw new ArgumentException("Invalid email format", nameof(value));
        }

        Value = value.Trim();
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);
}