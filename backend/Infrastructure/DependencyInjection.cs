using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Users.Interfaces;
using Application.Files.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Minio;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMinio(configuration).AddAuthServices(configuration);
        
        return services;
    }

    private static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetRequiredSection(nameof(MinioOptions)));
        services.AddScoped<IStorageProvider, MinioProvider>();

        return services;
    }

    private static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetRequiredSection(nameof(JwtOptions)));
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<SmtpOptions>(configuration.GetRequiredSection(nameof(SmtpOptions)));
        services.AddScoped<EmailSenderService>();
        services.AddScoped<IEmailVerificator, EmailVerificator>();
        
        return services;
    }
}