namespace Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty", nameof(value));
        }

        if (!value.Contains('@'))
        {
            throw new ArgumentException("Invalid email format", nameof(value));
        }

        Value = value.Trim().ToLower();
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);
}