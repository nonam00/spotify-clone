using System.Security.Cryptography;

using Application.Users.Interfaces;

namespace Infrastructure.Email;
 
public class EmailVerificator : IEmailVerificator
{
    private readonly EmailSenderService _emailSenderService;
    private readonly IConfirmationCodesRepository _confirmationCodesRepository;
    private readonly TimeSpan _codeExpiry;

    public EmailVerificator(
        EmailSenderService emailSenderService,
        IConfirmationCodesRepository confirmationCodesRepository)
    {
        _emailSenderService = emailSenderService;
        _confirmationCodesRepository = confirmationCodesRepository;
        _codeExpiry = TimeSpan.FromMinutes(15);
    }

    public string GenerateCode(int length = 6)
    {
        return RandomNumberGenerator.GetString("0123456789", length);
    }

    public async Task StoreCodeAsync(string email, string code)
    {
        await _confirmationCodesRepository.SetConfirmationCode(email, code, _codeExpiry).ConfigureAwait(false);
    }

    public async Task SendCodeAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        // hardcoded link, change to something else
        var confirmationLink = $"http://localhost:8080/1/auth/activate?email={email}&code={code}";
        var body = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>";
        await _emailSenderService
            .SendEmailAsync(email, "Account confirmation", body, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        var storedCode = await _confirmationCodesRepository
            .GetConfirmationCode(email)
            .ConfigureAwait(false);
        
        return storedCode is not null && storedCode == code;
    }
}