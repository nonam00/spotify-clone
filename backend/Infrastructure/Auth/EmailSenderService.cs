using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace Infrastructure.Auth;

public class EmailSenderService
{
    private readonly SmtpOptions _smtpOptions;

    public EmailSenderService(IOptions<SmtpOptions> smtpSettings)
    {
        _smtpOptions = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string email, string title, string body, CancellationToken cancellationToken)
    {
        var message = new MimeMessage
        {
            From = { new MailboxAddress("noreply", "sender@example.com") },
            To = { new MailboxAddress("User", email) },
            Subject = title,
            Body = new TextPart("html") { Text = body } 
        };
        
        using var client = new SmtpClient();
        
        await client.ConnectAsync(_smtpOptions.Server, _smtpOptions.Port, false, cancellationToken);
        var response = await client.SendAsync(message, cancellationToken);
        Console.WriteLine(response);
        await client.DisconnectAsync(true, cancellationToken);
    }
}