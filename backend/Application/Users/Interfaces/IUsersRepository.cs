using Domain;

namespace Application.Users.Interfaces;

public interface IUserRepository
{
    Task CreateUser(User user, CancellationToken cancellationToken = default);
    Task<User> GetUser(CancellationToken cancellationToken = default);
    Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default);
}