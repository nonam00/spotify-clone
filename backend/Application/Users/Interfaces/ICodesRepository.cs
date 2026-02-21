namespace Application.Users.Interfaces;

public interface ICodesRepository
{
    Task SetConfirmationCode(string email, string code, TimeSpan expiry);
    Task<string?> GetConfirmationCode(string email);
    Task SetRestoreCode(string email, string code, TimeSpan expiry);
    Task<string?> GetRestoreCode(string email);
}