using Application.Files.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Users.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Minio;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetRequiredSection(nameof(JwtOptions)));
        services.Configure<MinioOptions>(configuration.GetRequiredSection(nameof(MinioOptions)));
        services.AddMemoryCache();
        services.AddScoped<IStorageProvider, MinioProvider>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
            
        return services;
    }
}