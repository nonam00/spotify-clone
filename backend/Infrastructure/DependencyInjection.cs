using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Application.Interfaces;
using Application.Interfaces.Auth;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.Configure<AwsOptions>(configuration.GetSection(nameof(AwsOptions)));
            Console.WriteLine(configuration["AwsOptions"]);
            services.AddSingleton<IS3Provider, S3Provider>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}
