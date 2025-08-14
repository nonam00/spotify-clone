namespace Infrastructure.Auth;

public class SmtpOptions
{
    public string Server { get; init; } = null!;
    public int Port { get; init; }
}