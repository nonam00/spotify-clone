using Domain;

namespace Application.Users.Interfaces;

public interface IUsersRepository
{
    Task Add(User user, CancellationToken cancellationToken = default);
    Task<User?> Get(string email, CancellationToken cancellationToken = default);
    Task<User?> Get(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default);
}