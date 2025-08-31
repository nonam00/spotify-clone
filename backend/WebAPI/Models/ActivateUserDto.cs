using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Models;

[BindProperties]
public class ActivateUserDto
{
    [Required] public string Email { get; init; } = null!;
    [Required] public string Code { get; init; } = null!;
}