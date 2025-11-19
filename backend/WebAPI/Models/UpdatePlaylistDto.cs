using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UpdatePlaylistDto([Required] string Title, string? Description, string? ImageId);