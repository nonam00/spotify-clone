using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UpdatePlaylistDto
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
}