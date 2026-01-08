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
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddAuthServices(configuration)
                .AddEmailServices(configuration)
                .AddFilesServices(configuration);
        
            return services;
        }

        private IServiceCollection AddAuthServices(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetRequiredSection(nameof(JwtOptions)));
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }

        private IServiceCollection AddEmailServices(IConfiguration configuration)
        {
            services.Configure<SmtpOptions>(configuration.GetRequiredSection(nameof(SmtpOptions)));
            services.AddScoped<EmailSenderService>();
            services.AddScoped<ICodesClient, CodesClient>();
            return services;
        }

        private IServiceCollection AddFilesServices(IConfiguration configuration)
        {
            services.Configure<FileServiceOptions>(configuration.GetRequiredSection(nameof(FileServiceOptions)));
            services.AddScoped<IFileServiceClient, FileServiceClient>();
            return services;
        }
    }
}