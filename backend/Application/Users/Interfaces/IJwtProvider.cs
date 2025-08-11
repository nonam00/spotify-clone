using Domain;

namespace Application.Users.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}