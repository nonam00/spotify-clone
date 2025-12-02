namespace Application.Users.Interfaces;

public interface ICodesClient
{
    string GenerateCode(int length = 6);
    
    Task StoreConfirmationCodeAsync(string email, string code);
    Task SendConfirmationCodeAsync(string email, string code, CancellationToken cancellationToken = default);
    Task<bool> VerifyConfirmationCodeAsync(string email, string code);
    
    Task StoreRestoreTokenAsync(string email, string code);
    Task SendRestoreTokenAsync(string email, string code, CancellationToken cancellationToken = default);
    Task<bool> VerifyRestoreTokenAsync(string email, string code);
}