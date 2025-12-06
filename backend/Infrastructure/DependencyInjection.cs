using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Shared.Interfaces;
using Application.Users.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Email;
using Infrastructure.Files;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthServices(configuration)
            .AddEmailServices(configuration)
            .AddFilesServices();
        
        return services;
    }

    private static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetRequiredSection(nameof(JwtOptions)));
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }

    private static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetRequiredSection(nameof(SmtpOptions)));
        services.AddScoped<EmailSenderService>();
        services.AddScoped<ICodesClient, CodesClient>();
        return services;
    }

    private static IServiceCollection AddFilesServices(this IServiceCollection services)
    {
        services.AddScoped<IFileServiceClient, FileServiceClient>();
        return services;
    }
}