using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UpdateModeratorStatusDto([Required] bool IsActive);
