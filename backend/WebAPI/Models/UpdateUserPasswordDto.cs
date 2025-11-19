using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UpdateUserPasswordDto([Required] string CurrentPassword, [Required] string NewPassword);