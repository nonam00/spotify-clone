namespace Application.Users.Interfaces;

public interface IConfirmationCodesRepository
{
    Task SetConfirmationCode(string email, string code, TimeSpan expiry);

    Task<string?> GetConfirmationCode(string email);
}