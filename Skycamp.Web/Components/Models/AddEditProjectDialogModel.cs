using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class AddEditProjectDialogModel
{
    public Guid? Id { get; set; }

    public Guid WorkspaceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public bool IsAllAccess { get; set; }
}