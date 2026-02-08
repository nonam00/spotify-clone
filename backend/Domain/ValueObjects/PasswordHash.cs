namespace Domain.ValueObjects;

public record PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password hash cannot be empty", nameof(value));
        }

        Value = value;
    }

    public static implicit operator string(PasswordHash hash) => hash.Value;
    public static explicit operator PasswordHash(string value) => new(value);
}