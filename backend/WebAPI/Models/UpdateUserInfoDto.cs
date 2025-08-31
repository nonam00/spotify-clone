namespace WebAPI.Models;

public class UpdateUserInfoDto
{
    public string? FullName { get; set; }
    public IFormFile? Avatar { get; set; }
}