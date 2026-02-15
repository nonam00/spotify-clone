using FluentAssertions;

using Domain.Models;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Application.Users.Commands.UploadSong;
using Application.Users.Errors;
using Domain.Errors;

namespace Application.Tests.Users.Commands;

public class UploadSongCommandHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_ShouldCreateSong_WhenValid()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command = new UploadSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var song = await Context.Users
            .Where(u => u.Id == user.Id)
            .Include(u => u.UploadedSongs)
            .SelectMany(u => u.UploadedSongs)
            .FirstOrDefaultAsync();
        
        song.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldCreateUnpublishedSong_ByDefault()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        Context.ChangeTracker.Clear();
        
        var command = new UploadSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var song = await Context.Songs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UploaderId == user.Id);
        
        song.Should().NotBeNull();
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhemUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UploadSongCommand(
            user.Id,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.Empty,
            "Test Song",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleIsEmpty()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "",
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('a', 256);
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            longTitle,
            "Test Author",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenAuthorIsEmpty()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "",
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Author");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenAuthorExceedsMaxLength()
    {
        // Arrange
        var longAuthor = new string('a', 256);
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            longAuthor,
            "song.mp3",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Author");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongPathIsEmpty()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            "",
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Song path");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongPathExceedsMaxLength()
    {
        // Arrange
        var longPath = new string('a', 256);
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            longPath,
            "image.jpg");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Song path");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenImagePathIsEmpty()
    {
        // Arrange
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            "song.mp3",
            "");

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Image path");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenImagePathExceedsMaxLength()
    {
        // Arrange
        var longPath = new string('a', 256);
        var command = new UploadSongCommand(
            Guid.NewGuid(),
            "Test Song",
            "Test Author",
            "song.mp3",
            longPath);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Image path");
    }
}