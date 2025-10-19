using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class AddEditWorkspaceDialogModel
{
    public Guid? Id { get; set; }

    [Required]
    [Length(3, 100)]
    public string Name { get; set; } = null!;

    [Length(3, 500)]
    public string? Description { get; set; }
}