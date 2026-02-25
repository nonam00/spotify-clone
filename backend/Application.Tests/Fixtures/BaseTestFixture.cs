using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Domain.Common;

using Application.Moderators.Interfaces;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Integration;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;
using Persistence;
using Persistence.Repositories;

namespace Application.Tests.Fixtures;

/// <summary>
/// Configures in-process messaging members,
/// Mocks and configures external services,
/// Configures request and event handlers and validators
/// </summary>
public class BaseTestFixture
{
    protected readonly ServiceCollection Services;
    
    public Mock<IPasswordHasher> PasswordHasherMock { get; private set; } = null!;
    public Mock<IJwtProvider> JwtProviderMock { get; private set; } = null!;
    public Mock<IFileServicePublisher> FileServicePublisher { get; private set; } = null!;
    public Mock<IEmailServicePublisher> EmailServicePublisher { get; private set; } = null!;

    protected BaseTestFixture()
    {
        Services = [];
        
        ConfigureMocks();
        ConfigureCommonServices();
        ConfigureRepositories();
        ConfigureApplicationServices();
    }

    private void ConfigureMocks()
    {
        PasswordHasherMock = new Mock<IPasswordHasher>();
        JwtProviderMock = new Mock<IJwtProvider>();
        FileServicePublisher = new Mock<IFileServicePublisher>();
        EmailServicePublisher = new Mock<IEmailServicePublisher>();
        
        SetupPasswordHasherMocks();
        SetupJwtProviderMocks();
    }

    private void SetupPasswordHasherMocks()
    {
        PasswordHasherMock.Setup(x => x.Generate(It.IsAny<string>()))
            .Returns<string>(p => $"hashed_{p}");
        PasswordHasherMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((password, hash) => hash == $"hashed_{password}");
    }

    private void SetupJwtProviderMocks()
    {
        JwtProviderMock.Setup(x => x.GenerateUserToken(It.IsAny<Domain.Models.User>()))
            .Returns("mock_access_token");
        JwtProviderMock.Setup(x => x.GenerateModeratorToken(It.IsAny<Domain.Models.Moderator>()))
            .Returns("mock_moderator_token");
    }
    
    private void ConfigureCommonServices()
    {
        Services.AddSingleton(PasswordHasherMock.Object);
        Services.AddSingleton(JwtProviderMock.Object);
        Services.AddSingleton(EmailServicePublisher.Object);
        Services.AddSingleton(FileServicePublisher.Object);
        
        Services.AddScoped(typeof(ILoggerFactory), typeof(LoggerFactory));
        Services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
    }

    private void ConfigureRepositories()
    {
        Services.AddScoped<IUsersRepository, UsersRepository>();
        Services.AddScoped<ISongsRepository, SongsRepository>();
        Services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
        Services.AddScoped<IModeratorsRepository, ModeratorsRepository>();
        Services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        
        // Mocked redis repository
        var codesRepositoryMock = new Mock<ICodesRepository>();
        Services.AddSingleton(codesRepositoryMock.Object);
    }

    private void ConfigureApplicationServices()
    {
        Services.AddScoped<IUnitOfWork, UnitOfWork>();
        Services.AddScoped<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();
        
        Services.AddApplication();
    }

    protected ServiceProvider BuildServiceProvider() => Services.BuildServiceProvider();
}