using Application.Files.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Users.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Aws;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetRequiredSection(nameof(JwtOptions)));
        services.Configure<AwsOptions>(configuration.GetRequiredSection(nameof(AwsOptions)));
        services.AddScoped<IStorageProvider, S3Provider>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
            
        return services;
    }
}