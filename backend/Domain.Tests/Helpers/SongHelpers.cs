using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Tests.Helpers;

public static class SongHelpers
{
    public static Song CreateSong(bool isPublished = false)
    {
        var song = Song.Create(
            "Title",
            "Author",
            new FilePath("audio.mp3"),
            new FilePath("image.png")).Value;
        
        if (isPublished)
        {
            song.Publish();
        }

        return song;
    }

    public static List<Song> CreateSongList(int size, bool isPublished = true)
    {
        var list = new List<Song>(capacity: size);
        
        for (var i = 0; i < size; i++)
        {
            var song = CreateSong(isPublished: isPublished);
            list.Add(song);
        }
        
        return list;
    }
}