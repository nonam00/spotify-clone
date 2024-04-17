using Application.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = $"{configuration.GetConnectionString("PostgresDb")}Password={configuration["DbPassword"]}";

            services.AddDbContext<SongsDbContext>(options =>
            {
                options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<ISongsDbContext>(provider =>
                provider.GetService<SongsDbContext>());

            return services;
        }
    }
}
