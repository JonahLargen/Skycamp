using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class AddEditWorkspaceDialogModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }
}