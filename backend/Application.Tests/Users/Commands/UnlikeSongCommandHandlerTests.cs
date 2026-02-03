using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Users.Commands.UnlikeSong;
using Application.Users.Errors;

namespace Application.Tests.Users.Commands;

public class UnlikeSongCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldUnlikeSong_WhenSongIsLiked()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);        
        
        user.LikeSong(song);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UnlikeSongCommand(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var likedSong = await Context.Set<LikedSong>()
            .AsNoTracking()
            .SingleOrDefaultAsync(ls => ls.UserId == user.Id && ls.SongId == song.Id);
        
        likedSong.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UnlikeSongCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var user = User.Create(new Email("user@mail.com"), new PasswordHash("hashed_password"), "USer");
        user.Activate();
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        moderator.DeactivateUser(user);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UnlikeSongCommand(user.Id, song.Id);
        
        // Act
        var result = await Mediator.Send(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.NotActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSongNotLiked()
    {
        // Arrange
        var user = User.Create(new Email("test@example.com"), new PasswordHash("hashed_password"), "User");
        user.Activate();
        
        var song = user.UploadSong("Song", "Author", new FilePath("song1.mp3"), new FilePath("img1.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);        
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new UnlikeSongCommand(user.Id, song.Id);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserDomainErrors.SongNotLiked);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new UnlikeSongCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSongIdIsEmpty()
    {
        // Arrange
        var command = new UnlikeSongCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await Mediator.Send(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("SongId");
    }
}
