using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Skycamp.ApiService.Data.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    [MaxLength(256)]
    public string? AvatarUrl { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginUtc { get; set; }
}