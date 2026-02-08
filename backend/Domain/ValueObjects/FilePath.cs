namespace Domain.ValueObjects;

public record FilePath
{
    public string Value { get; }

    public FilePath(string? value)
    {
        Value = value ?? string.Empty;
    }

    public static implicit operator string(FilePath path) => path.Value;
    public static explicit operator FilePath(string value) => new(value);
}