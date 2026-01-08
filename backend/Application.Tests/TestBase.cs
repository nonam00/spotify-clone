using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

using Persistence;
using Persistence.Repositories;
using Application.Shared.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Application.Songs.Interfaces;
using Application.Playlists.Interfaces;
using Application.Moderators.Interfaces;
using Infrastructure.Email;
using Domain.Common;

namespace Application.Tests;

public abstract class TestBase : IDisposable
{
    protected IMediator Mediator { get; }
    protected AppDbContext Context { get; }
    protected IServiceProvider ServiceProvider { get; }
    protected Mock<IPasswordHasher> PasswordHasherMock { get; }
    protected Mock<IJwtProvider> JwtProviderMock { get; }
    protected Mock<IFileServiceClient> FileServiceClientMock { get; }
    protected Mock<ILoggerFactory> LoggerFactoryMock { get; }

    protected TestBase()
    {
        var services = new ServiceCollection();
        
        // In-memory db configuration
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            options.ConfigureWarnings(w =>
                w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
        });

        // Configuring mock services 
        PasswordHasherMock = new Mock<IPasswordHasher>();
        JwtProviderMock = new Mock<IJwtProvider>();
        FileServiceClientMock = new Mock<IFileServiceClient>();
        LoggerFactoryMock = new Mock<ILoggerFactory>();
        var codesClientMock = new Mock<ICodesClient>();
        
        PasswordHasherMock.Setup(x => x.Generate(It.IsAny<string>()))
            .Returns<string>(p => $"hashed_{p}");
        PasswordHasherMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((password, hash) => hash == $"hashed_{password}");
        
        JwtProviderMock.Setup(x => x.GenerateUserToken(It.IsAny<Domain.Models.User>()))
            .Returns("mock_access_token");
        JwtProviderMock.Setup(x => x.GenerateModeratorToken(It.IsAny<Domain.Models.Moderator>()))
            .Returns("mock_moderator_token");

        var loggerMock = new Mock<ILogger>();
        LoggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);

        // Configuring mocks for ICodesClient
        codesClientMock.Setup(x => x.VerifyConfirmationCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        codesClientMock.Setup(x => x.VerifyRestoreTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        codesClientMock.Setup(x => x.GenerateCode(It.IsAny<int>()))
            .Returns("123456");
        
        services.AddSingleton(PasswordHasherMock.Object);
        services.AddSingleton(JwtProviderMock.Object);
        services.AddSingleton(FileServiceClientMock.Object);
        services.AddSingleton(LoggerFactoryMock.Object);
        services.AddSingleton(codesClientMock.Object);
        services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
        services.AddScoped<IModeratorsRepository, ModeratorsRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        
        // Mock for Redis codes repository
        var codesRepositoryMock = new Mock<ICodesRepository>();
        codesRepositoryMock.Setup(x => x.SetConfirmationCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);
        codesRepositoryMock.Setup(x => x.GetConfirmationCode(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        codesRepositoryMock.Setup(x => x.SetRestoreCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);
        codesRepositoryMock.Setup(x => x.GetRestoreCode(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        services.AddSingleton(codesRepositoryMock.Object);
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();
        
        services.AddApplication();
        
        ServiceProvider = services.BuildServiceProvider();
        
        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        Context = ServiceProvider.GetRequiredService<AppDbContext>();
        
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
