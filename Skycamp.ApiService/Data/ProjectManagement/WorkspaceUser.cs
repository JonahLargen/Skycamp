using Skycamp.ApiService.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.ProjectManagement;

[Table("WorkspaceUsers", Schema = "projectmgmt")]
public class WorkspaceUser
{
    [Key]
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public string UserId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public DateTime JoinedUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(WorkspaceId))]
    public Workspace Workspace { get; set; } = null!;

    [ForeignKey(nameof(RoleName))]
    public WorkspaceRole Role { get; set; } = null!;
}