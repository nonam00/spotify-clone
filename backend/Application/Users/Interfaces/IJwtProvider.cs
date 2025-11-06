using Domain.Models;

namespace Application.Users.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}