using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;
using Application.Moderators.Interfaces;
using Infrastructure.Email;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddDatabases(configuration).AddRepositories();
            return services;
        }

        private IServiceCollection AddDatabases(IConfiguration configuration)
        {
            var postgresConnectionString = configuration.GetConnectionString("Postgres") 
                ?? throw new NullReferenceException("Postgres connection string is null");
        
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(postgresConnectionString)
                    .UseSnakeCaseNamingConvention();
            
                options.ConfigureWarnings(c =>
                {
                    c.Ignore(RelationalEventId.PendingModelChangesWarning);
                });
            });
        
            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new NullReferenceException("Redis connection string is null");
                
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));
        
            return services;
        }

        private IServiceCollection AddRepositories()
        {
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ISongsRepository, SongsRepository>();
            services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
            services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
            services.AddScoped<IModeratorsRepository, ModeratorsRepository>();

            services.AddScoped<ICodesRepository, CodesRepository>();
        
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        
            return services;
        }
    }
}