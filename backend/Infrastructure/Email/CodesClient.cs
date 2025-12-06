using System.Security.Cryptography;

using Application.Users.Interfaces;

namespace Infrastructure.Email;
 
public class CodesClient : ICodesClient
{
    private readonly EmailSenderService _emailSenderService;
    private readonly ICodesRepository _codesRepository;
    private readonly TimeSpan _codeExpiry;

    public CodesClient(EmailSenderService emailSenderService, ICodesRepository codesRepository)
    {
        _emailSenderService = emailSenderService;
        _codesRepository = codesRepository;
        _codeExpiry = TimeSpan.FromMinutes(15);
    }

    public string GenerateCode(int length = 6)
    {
        return RandomNumberGenerator.GetString("0123456789", length);
    }

    public async Task StoreConfirmationCodeAsync(string email, string code)
    {
        await _codesRepository.SetConfirmationCode(email, code, _codeExpiry).ConfigureAwait(false);
    }

    public async Task SendConfirmationCodeAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        // hardcoded link, change to something else
        var confirmationLink = $"http://localhost/api/1/auth/activate?email={email}&code={code}";
        var body = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>";
        await _emailSenderService
            .SendEmailAsync(email, "Account confirmation", body, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> VerifyConfirmationCodeAsync(string email, string code)
    {
        var storedCode = await _codesRepository
            .GetConfirmationCode(email)
            .ConfigureAwait(false);
        
        return storedCode is not null && storedCode == code;
    }

    public async Task StoreRestoreTokenAsync(string email, string code)
    {
        await _codesRepository.SetRestoreCode(email, code, _codeExpiry).ConfigureAwait(false);
    }

    public async Task SendRestoreTokenAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        // hardcoded link, change to something else
        var confirmationLink = $"http://localhost/api/1/auth/restore-access?email={email}&code={code}";
        var body =
            $"Please restore access to your account by clicking <a href='{confirmationLink}'>here</a>. " +
            "We've changed your password to <b>12345678</b> so you can use it to change your password in your account settings.";
        await _emailSenderService
            .SendEmailAsync(email, "Account restore", body, cancellationToken)
            .ConfigureAwait(false);   
    }

    public async Task<bool> VerifyRestoreTokenAsync(string email, string code)
    {
        var storedCode = await _codesRepository
            .GetRestoreCode(email)
            .ConfigureAwait(false);
        
        return storedCode is not null && storedCode == code;
    }
}