using Application.Moderators.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;
using Infrastructure.Email;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabases(configuration).AddRepositories();
        return services;
    }

    private static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
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

    private static IServiceCollection AddRepositories(this IServiceCollection services)
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