using Domain.Models;
using Application.Users.Models;

namespace Application.Users.Interfaces;

public interface IUsersRepository
{
    Task Add(User user, CancellationToken cancellationToken = default);
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRefreshTokens(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithUploadedSongs(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithLikedSongs(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithPlaylists(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRefreshTokens(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenValue(string refreshTokenValue, CancellationToken cancellationToken = default);
    Task<bool> CheckIfSongLiked(Guid userId, Guid songId, CancellationToken cancellationToken = default);
    Task<List<UserVm>> GetListVm(CancellationToken cancellationToken = default);
    Task<List<User>> GetNonActiveList(CancellationToken cancellationToken = default);
    void Update(User user);
    void DeleteRange(IEnumerable<User> users);
}