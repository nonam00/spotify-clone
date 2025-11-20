using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record CreateModeratorDto(
    [Required, EmailAddress] string Email,
    [Required] string FullName,
    [Required] string Password,
    [Required] bool IsSuper);
    