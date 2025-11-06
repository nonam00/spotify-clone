using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UpdateUserPasswordDto
{
    [Required] public string CurrentPassword { get; init; } = null!;
    [Required] public string NewPassword { get; init; } = null!;
}