
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Domain.Common;

using Application.Moderators.Interfaces;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;

using Persistence;
using Persistence.Repositories;

namespace Application.Tests;

public class TestBaseWithPostgresFixture : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    protected IMediator Mediator { get; }
    protected AppDbContext Context { get; }
    protected Mock<IPasswordHasher> PasswordHasherMock { get; }

    protected TestBaseWithPostgresFixture(PostgreSqlFixture fixture)
    {
        var services = new ServiceCollection();
        
        // Test container postgres configuration
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(fixture.ConnectionString)
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging();
            options.ConfigureWarnings(c =>
                c.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        // Configuring mock services 
        PasswordHasherMock = new Mock<IPasswordHasher>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var fileServiceClientMock = new Mock<IFileServiceClient>();
        var codesClientMock = new Mock<ICodesClient>();
        
        PasswordHasherMock.Setup(x => x.Generate(It.IsAny<string>()))
            .Returns<string>(p => $"hashed_{p}");
        PasswordHasherMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((password, hash) => hash == $"hashed_{password}");
        
        jwtProviderMock.Setup(x => x.GenerateUserToken(It.IsAny<Domain.Models.User>()))
            .Returns("mock_access_token");
        jwtProviderMock.Setup(x => x.GenerateModeratorToken(It.IsAny<Domain.Models.Moderator>()))
            .Returns("mock_moderator_token");

        // Configuring mocks for ICodesClient
        codesClientMock.Setup(x => x.VerifyConfirmationCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        codesClientMock.Setup(x => x.VerifyRestoreTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        codesClientMock.Setup(x => x.GenerateCode(It.IsAny<int>()))
            .Returns("123456");
        
        services.AddSingleton(PasswordHasherMock.Object);
        services.AddSingleton(jwtProviderMock.Object);
        services.AddSingleton(codesClientMock.Object);
        services.AddSingleton(fileServiceClientMock.Object);
        
        services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
        services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

        services.AddApplication();
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
        services.AddScoped<IModeratorsRepository, ModeratorsRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        Mediator = serviceProvider.GetRequiredService<IMediator>();
        Context = serviceProvider.GetRequiredService<AppDbContext>();

        Context.Database.EnsureCreated();
    }

    public async Task InitializeAsync()
    {
        await Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE moderators, users, songs CASCADE;");
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}   