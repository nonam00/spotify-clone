using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class CreateSongDto
{
    [Required] public string Title { get; init; } = null!;
    [Required] public string Author { get; init; } = null!;
    [Required] public Guid ImageId { get; init; }
    [Required] public Guid AudioId{ get; init; }
}