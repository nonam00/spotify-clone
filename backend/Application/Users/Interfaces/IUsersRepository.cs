using Domain;
using Application.Users.Models;

namespace Application.Users.Interfaces;

public interface IUsersRepository
{
    Task Add(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default);
    Task<bool> CheckIfActivated(Guid id, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
}