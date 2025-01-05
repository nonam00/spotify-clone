using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Application.Interfaces;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // connection string from api public configuration
            var connectionString = configuration.GetConnectionString("PostgresDb")
                ?? throw new NullReferenceException("PostgresDb connection string is null");
            // password for database from user secret or env file
            var dbPassword = configuration["DbPassword"]
                ?? throw new NullReferenceException("DbPassword string is null");

            services.AddDbContext<SongsDbContext>(options =>
            {
                options.UseNpgsql(connectionString + dbPassword)
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<ISongsDbContext>(provider =>
                provider.GetRequiredService<SongsDbContext>());

            return services;
        }
    }
}
