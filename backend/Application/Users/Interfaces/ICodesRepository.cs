namespace Application.Users.Interfaces;

public interface ICodesRepository
{
    Task<bool> SetConfirmationCode(string email, string code, TimeSpan expiry);
    Task<string> GetConfirmationCode(string email);
    Task<bool> SetRestoreCode(string email, string code, TimeSpan expiry);
    Task<string> GetRestoreCode(string email);
}