namespace Application.Users.Interfaces;

public interface IEmailVerificator
{
    string GenerateCode(int length = 6);
    Task StoreCodeAsync(string email, string code);
    Task SendCodeAsync(string email, string code, CancellationToken cancellationToken = default);
    Task<bool> VerifyCodeAsync(string email, string code);
}