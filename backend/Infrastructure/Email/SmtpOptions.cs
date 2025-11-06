namespace Infrastructure.Email;

public class SmtpOptions
{
    public string Server { get; init; } = null!;
    public int Port { get; init; }
}