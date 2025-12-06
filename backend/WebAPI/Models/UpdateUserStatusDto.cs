using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UpdateUserStatusDto([Required] bool IsActive);