using Domain.Models;

namespace Application.Shared.Interfaces;

public interface IJwtProvider
{
    string GenerateUserToken(User user);
    string GenerateModeratorToken(Moderator moderator);
}