using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UserCredentialsDto
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}