using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Commands.PublishSong;
using Application.Moderators.Errors;
using Application.Songs.Errors;

namespace Application.Tests.Moderators.Commands;

public class PublishSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldPublishSong_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new PublishSongCommand(moderator.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var publishedSong = await Context.Songs.FirstOrDefaultAsync(s => s.Id == song.Id);
        
        publishedSong.Should().NotBeNull();
        publishedSong.IsPublished.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorDoesNotExist()
    {   
        // Arrange
        var command = new PublishSongCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorIsNotActive()
    {
        // Arrange
        var admin = Moderator.Create(
            new Email("admin@admin.com"),
            new PasswordHash("hashed_password1"),
            "Admin",
            ModeratorPermissions.CreateSuperAdmin());
        
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        admin.DeactivateModerator(moderator);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var command = new PublishSongCommand(moderator.Id, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorDoesntHavePermission()
    {
        // Arrange
        var permissions = new ModeratorPermissions(
            canManageContent: false,
            canManageModerators: false,
            canManageUsers: true,
            canViewReports: true);
        
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            permissions);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var command = new PublishSongCommand(moderator.Id, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotFound()
    {
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();
        
        var command = new PublishSongCommand(moderator.Id, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongIsAlreadyPublished()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        
        moderator.PublishSong(song);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new PublishSongCommand(moderator.Id, song.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongDomainErrors.AlreadyPublished);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongIsMarkedForDeletion()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var song = Song.Create("Song", "Author", new FilePath("song.mp3"), new FilePath("image.jpg"));
        
        moderator.DeleteSong(song);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Songs.AddAsync(song);
        await Context.SaveChangesAsync();
        
        var command = new PublishSongCommand(moderator.Id, song.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongDomainErrors.CannotPublishMarkedForDeletion);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorIdIsEmpty()
    {
        // Arrange
        var command = new PublishSongCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ModeratorId");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongIdIsEmpty()
    {
        // Arrange
        var command = new PublishSongCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
