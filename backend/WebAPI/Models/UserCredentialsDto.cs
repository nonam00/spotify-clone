using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UserCredentialsDto
{
    [Required, EmailAddress] public string Email { get; init; } = null!;
    [Required] public string Password { get; init; } = null!;
}