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
            // getting connection string from api configuration and the password for database from user secret file
            var connectionString = configuration.GetConnectionString("PostgresDb") + configuration["DbPassword"];

            services.AddDbContext<SongsDbContext>(options =>
            {
                options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<ISongsDbContext>(provider =>
                provider.GetService<SongsDbContext>()
                ?? throw new Exception("Can't get db context service"));

            return services;
        }
    }
}
