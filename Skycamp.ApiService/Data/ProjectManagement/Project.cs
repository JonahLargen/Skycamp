using Skycamp.ApiService.Data.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("Projects", Schema = "projectmgmt")]
public class Project
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DefaultValue(false)]
    public bool IsAllAccess { get; set; } = false;

    public string? CreateUserId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CreateUserId))]
    public ApplicationUser? CreateUser { get; set; } = null!;
}
