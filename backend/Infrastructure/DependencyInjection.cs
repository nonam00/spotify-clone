using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Shared.Integration;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

using Infrastructure.Auth;
using Infrastructure.Email;
using Infrastructure.Files;
using Infrastructure.Messaging;
using Infrastructure.Transcription;

namespace Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddAuthServices(configuration)
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

        private void AddMessagingServices(IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetRequiredSection(nameof(RabbitMqOptions)));
            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<RabbitMqTopologyInitializer>();
            services.AddHostedService<RabbitMqTopologyInitializerHostedService>();
            
            services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
            
            services.AddScoped<IFileServicePublisher, FileServicePublisher>();
            services.AddScoped<IEmailServicePublisher, EmailServicePublisher>();
            services.AddScoped<ITranscriptionServicePublisher, TranscriptionServicePublisher>();
            
            services.AddHostedService<UpdateSongInformationConsumer>();
        }
    }
}