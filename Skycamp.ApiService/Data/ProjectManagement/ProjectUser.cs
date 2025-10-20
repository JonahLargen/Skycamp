using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("ProjectUsers", Schema = "projectmgmt")]
public class ProjectUser
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string UserId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public DateTime JoinedUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; } = null!;

    [ForeignKey(nameof(RoleName))]
    public ProjectRole Role { get; set; } = null!;
}