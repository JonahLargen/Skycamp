using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class AddEditTodoDialogModel
{
    public Guid? Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid WorkspaceId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    public string? PrimaryAssigneeId { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public List<ProjectUser> ProjectUsers { get; set; } = [];
}

public class ProjectUser
{
    public string Id { get; set; } = null!;
    public string? DisplayName { get; set; }
}
