using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UserCredentialsDto([Required, EmailAddress] string Email, [Required] string Password);