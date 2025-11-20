using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record UpdateModeratorPermissionsDto(
    [Required] bool CanManageUsers,
    [Required] bool CanManageContent,
    [Required] bool CanViewReports,
    [Required] bool CanManageModerators );
