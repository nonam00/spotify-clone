using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class CreateSongDto
{
    [Required] public string Title { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
    [Required] public Guid ImageId { get; set; }
    [Required] public Guid AudioId{ get; set; }
}