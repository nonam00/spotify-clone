using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Users.Interfaces;
using Infrastructure.Auth;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthServices(configuration);
        
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