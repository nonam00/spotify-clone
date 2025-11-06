using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email;

public class EmailSenderService : IDisposable, IAsyncDisposable
{
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IOptions<SmtpOptions> smtpSettings, ILogger<EmailSenderService> logger)
    {
        _logger = logger;
        _smtpClient = new SmtpClient();
        var connectTask = _smtpClient.ConnectAsync(smtpSettings.Value.Server, smtpSettings.Value.Port, false);
        connectTask.Wait();
    }

    public async Task SendEmailAsync(string email, string title, string body, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending email to {email}", email);
        var message = new MimeMessage
        {
            From = { new MailboxAddress("noreply", "sender@example.com") },
            To = { new MailboxAddress("User", email) },
            Subject = title,
            Body = new TextPart("html") { Text = body } 
        };
        
        var response = await _smtpClient.SendAsync(message, cancellationToken);
        _logger.LogDebug("Email sent, received response: {response}" , response);
    }

    public void Dispose()
    {
        _smtpClient.Disconnect(true);
        _smtpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _smtpClient.DisconnectAsync(true);
        _smtpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}