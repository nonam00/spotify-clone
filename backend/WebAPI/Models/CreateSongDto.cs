using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class CreateSongDto
{
    [Required] public string Title { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
    [Required] public string SongPath { get; set; } = null!;
    [Required] public string ImagePath { get; set; } = null!;
}