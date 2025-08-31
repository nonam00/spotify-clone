using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UpdateUserPasswordDto
{
    [Required] public string CurrentPassword { get; set; } = null!;
    [Required] public string NewPassword { get; set; } = null!;
}