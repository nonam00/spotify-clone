using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class CreateSongDto
{
    [Required] public string Title { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
    [Required] public IFormFile Audio{ get; set; } = null!;
    [Required] public IFormFile Image { get; set; } = null!;
}