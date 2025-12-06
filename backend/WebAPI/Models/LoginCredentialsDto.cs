using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record LoginCredentialsDto([Required, EmailAddress] string Email, [Required] string Password);