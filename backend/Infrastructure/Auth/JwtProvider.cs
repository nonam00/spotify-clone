using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Domain.Models;
using Application.Shared.Interfaces;

namespace Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateUserToken(User user)
    {
        Claim[] claims = 
        [
            new (CustomClaims.UserId, user.Id.ToString()),
            new (CustomClaims.EntityType, EntityTypes.User)
        ];
        
        return GenerateToken(claims);
    }

    public string GenerateModeratorToken(Moderator moderator)
    {
        List<Claim> claims =
        [
            new (CustomClaims.ModeratorId, moderator.Id.ToString()),
            new (CustomClaims.EntityType, EntityTypes.Moderator)
        ];

        if (moderator.Permissions.CanManageUsers)
        {
            claims.Add(new Claim(CustomClaims.Permission, Permissions.ManageUsers));
        }

        if (moderator.Permissions.CanManageContent)
        {
            claims.Add(new Claim(CustomClaims.Permission, Permissions.ManageContent));
        }

        if (moderator.Permissions.CanManageModerators)
        {
            claims.Add(new Claim(CustomClaims.Permission, Permissions.ManageModerators));
        }

        if (moderator.Permissions.CanViewReports)
        {
            claims.Add(new Claim(CustomClaims.Permission, Permissions.ViewReports));
        }

        return GenerateToken(claims.ToArray());
    }
    
    private string GenerateToken(Claim[] claims)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}