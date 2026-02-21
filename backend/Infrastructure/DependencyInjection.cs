using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Shared.Integration;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

using Infrastructure.Auth;
using Infrastructure.Email;
using Infrastructure.Files;
using Infrastructure.Messaging;

namespace Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddAuthServices(configuration)
                .AddEmailServices(configuration)
                .AddMessagingServices(configuration);

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

        private IServiceCollection AddMessagingServices(IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetRequiredSection(nameof(RabbitMqOptions)));
            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
            
            services.AddScoped<IFileServicePublisher, FileServicePublisher>();
            
            return services;
        }
    }
}