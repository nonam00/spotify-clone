using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Commands.DeleteSongs;
using Application.Moderators.Errors;
using Application.Songs.Errors;

namespace Application.Tests.Moderators.Commands;

public class DeleteSongsCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldDeleteSongs_WhenValid()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var user = User.Create(new Email("user@emal.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        // Clear tracking to avoid conflicts
        Context.ChangeTracker.Clear();
        
        var command = new DeleteSongsCommand(moderator.Id,  [song1.Id, song2.Id]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var publishedSongs = await Context.Songs
            .AsNoTracking()
            .Where(s => s.MarkedForDeletion)
            .ToListAsync(CancellationToken.None);
        
        publishedSongs.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModeratorDoesNotExist()
    {   
        // Arrange
        var command = new DeleteSongsCommand(Guid.NewGuid(), [Guid.NewGuid()]);

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
        
        var command = new DeleteSongsCommand(moderator.Id, [Guid.NewGuid()]);

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
        
        var command = new DeleteSongsCommand(moderator.Id, [Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }
    
    [Fact] 
    public async Task Handle_ShouldReturnFailure_WhenSomeSongsNotFound()
    {
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());

        var user = User.Create(new Email("user@emal.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new DeleteSongsCommand(moderator.Id, [song1.Id, Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongErrors.SongsNotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongIsPublished()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var user = User.Create(new Email("user@emal.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;

        moderator.PublishSong(song1);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new DeleteSongsCommand(moderator.Id, [song1.Id, song2.Id]);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongDomainErrors.CannotDeletePublished);
    }    
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongIsAlreadyMarkedForDeletion()
    {
        // Arrange
        var moderator = Moderator.Create(
            new Email("moderator@example.com"),
            new PasswordHash("hashed_password1"),
            "Moderator",
            ModeratorPermissions.CreateDefault());
        
        var user = User.Create(new Email("user@emal.com"), new PasswordHash("hashed_password"));
        user.Activate();
        
        var song1 = user.UploadSong("Song 1", "Author 1", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        var song2 = user.UploadSong("Song 2", "Author 2", new FilePath("song.mp3"), new FilePath("image.jpg")).Value;
        
        moderator.DeleteSong(song1);
        
        await Context.Moderators.AddAsync(moderator);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new DeleteSongsCommand(moderator.Id, [song1.Id, song2.Id]);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SongDomainErrors.AlreadyMarkedForDeletion);
    }    

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenModeratorIdIsEmpty()
    {
        // Arrange
        var command = new DeleteSongsCommand(Guid.Empty, [Guid.NewGuid()]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("ModeratorId");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongIdListIsEmpty()
    {
        // Arrange
        var command = new DeleteSongsCommand(Guid.NewGuid(), []);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongIds");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSomeSongIdIsEmpty()
    {
        // Arrange
        var command = new DeleteSongsCommand(Guid.NewGuid(), [Guid.Empty]);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Song ID");
    }
}
