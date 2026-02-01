using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UploadSongDto(
    [Required] string Title,
    [Required] string Author,
    [Required] Guid ImageId,
    [Required] Guid AudioId);