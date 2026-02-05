using Domain.Errors;
using Domain.Events;
using Domain.Models;
using Domain.Tests.Helpers;
using FluentAssertions;

namespace Domain.Tests.Moderators;

public class ModeratorSongTests
{
    private readonly Moderator _activeModerator = ModeratorHelpers.CreateModerator(isActive: true);
    private readonly Moderator _inactiveModerator = ModeratorHelpers.CreateModerator(isActive: false);
    
    [Fact]
    public void PublishSong_WhenModeratorIsActiveAndHasPermissions_ShouldPublishSong()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        song.IsPublished.Should().BeFalse();

        // Act
        var result = _activeModerator.PublishSong(song);

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void PublishSong_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong();

        // Act
        var result = _inactiveModerator.PublishSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void PublishSong_WhenModeratorCannotManageContent_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canViewReports: true,
            canManageContent: false,
            canManageUsers: true);
            
        var song = SongHelpers.CreateSong();

        // Act
        var result = moderatorWithoutPermissions.PublishSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void PublishSongs_WhenAllConditionsMet_ShouldPublishAllSongs()
    {
        // Arrange
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };
        
        // Act
        var result = _activeModerator.PublishSongs(songs);

        // Assert
        result.IsSuccess.Should().BeTrue();
        songs.ForEach(s => s.IsPublished.Should().BeTrue());
    }

    [Fact]
    public void PublishSongs_WithEmptyList_ShouldReturnFailure()
    {
        // Arrange
        var songs = new List<Song>();

        // Act
        var result = _activeModerator.PublishSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageEmptySongList);
    }

    [Fact]
    public void PublishSongs_WhenAnySongFailsToPublish_ShouldReturnFailure()
    {
        // Arrange
        var song1 = SongHelpers.CreateSong();
        var song2 = SongHelpers.CreateSong();
        
        song2.MarkForDeletion(); // Mark for deletion, which will cause publish to fail
            
        var songs = new List<Song> { song1, song2 };

        // Act
        var result = _activeModerator.PublishSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        song1.IsPublished.Should().BeFalse();
        song2.IsPublished.Should().BeFalse();
    }
    
    [Fact]
    public void PublishSongs_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };
        
        // Act
        var result = _inactiveModerator.PublishSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public void PublishSongs_WhenModeratorCannotManageContent_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canManageContent: false,
            isActive: true);
            
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };
        
        // Act
        var result = moderatorWithoutPermissions.PublishSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }

    [Fact]
    public void UnpublishSong_WhenAllConditionsMet_ShouldUnpublishSong()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);

        // Act
        var result = _activeModerator.UnpublishSong(song);

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void UnpublishSong_WhenSongIsAlreadyUnpublished_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong();

        // Act
        var result = _activeModerator.UnpublishSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        song.IsPublished.Should().BeFalse();
    }
    
    [Fact]
    public void UnpublishSong_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);
        
        // Act
        var result = _inactiveModerator.UnpublishSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public void UnpublishSong_WhenModeratorCannotManageContent_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canManageContent: false,
            isActive: true);
            
        var song = SongHelpers.CreateSong(isPublished: true);
        
        // Act
        var result = moderatorWithoutPermissions.UnpublishSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }

    [Fact]
    public void DeleteSong_WhenAllConditionsMet_ShouldMarkSongForDeletion()
    {
        // Arrange
        var song = SongHelpers.CreateSong();
        song.MarkedForDeletion.Should().BeFalse();

        // Act
        var result = _activeModerator.DeleteSong(song);

        // Assert
        result.IsSuccess.Should().BeTrue();
        song.MarkedForDeletion.Should().BeTrue();
        _activeModerator.DomainEvents.Should().ContainSingle(e => e is ModeratorDeletedSongEvent);
    }

    [Fact]
    public void DeleteSong_WhenSongIsPublished_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: true);

        // Act
        var result = _activeModerator.DeleteSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SongDomainErrors.CannotDeletePublished);
        song.MarkedForDeletion.Should().BeFalse();
        _activeModerator.DomainEvents.Should().BeEmpty();
    }
    
    [Fact]
    public void DeleteSong_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var song = SongHelpers.CreateSong(isPublished: false);
        
        // Act
        var result = _inactiveModerator.DeleteSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public void DeleteSong_WhenModeratorCannotManageContent_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canManageContent: false,
            isActive: true);
            
        var song = SongHelpers.CreateSong(isPublished: false);
        
        // Act
        var result = moderatorWithoutPermissions.DeleteSong(song);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }
    

    [Fact]
    public void DeleteSongs_WhenAllConditionsMet_ShouldMarkAllSongsForDeletion()
    {
        // Arrange
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };

        // Act
        var result = _activeModerator.DeleteSongs(songs);

        // Assert
        result.IsSuccess.Should().BeTrue();
        songs.ForEach(s => s.MarkedForDeletion.Should().BeTrue());
        _activeModerator.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void DeleteSongs_WithEmptyList_ShouldReturnFailure()
    {
        // Arrange
        var songs = new List<Song>();

        // Act
        var result = _activeModerator.DeleteSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageEmptySongList);
    }

    [Fact]
    public void DeleteSongs_WhenAnySongFailsToDelete_ShouldReturnFailure()
    {
        // Arrange
        var song1 = SongHelpers.CreateSong();
        var song2 = SongHelpers.CreateSong(isPublished: true); // Published, can't delete
            
        var songs = new List<Song> { song1, song2 };

        // Act
        var result = _activeModerator.DeleteSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        song1.MarkedForDeletion.Should().BeFalse();
        song2.MarkedForDeletion.Should().BeFalse();
        _activeModerator.DomainEvents.Should().BeEmpty();
    }
    
    [Fact]
    public void DeleteSongs_WhenModeratorIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };    
        
        // Act
        var result = _inactiveModerator.DeleteSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.NotActive);
    }
    
    [Fact]
    public void DeleteSongs_WhenModeratorCannotManageContent_ShouldReturnFailure()
    {
        // Arrange
        var moderatorWithoutPermissions = ModeratorHelpers.CreateModerator(
            canManageContent: false,
            isActive: true);
            
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong(),
            SongHelpers.CreateSong()
        };
        
        // Act
        var result = moderatorWithoutPermissions.DeleteSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ModeratorDomainErrors.CannotManageContent);
    }
}