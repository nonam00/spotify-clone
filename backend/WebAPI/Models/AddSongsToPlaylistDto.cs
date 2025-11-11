using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class AddSongsToPlaylistDto
{
    [Required] public List<Guid> SongIds { get; init; } = null!;
}