using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UpdatePlaylistDto
{
    [Required] public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? ImageId { get; init; }
}