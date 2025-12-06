using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UserRegistrationDto([Required, EmailAddress] string Email, [Required] string Password, string? FullName);