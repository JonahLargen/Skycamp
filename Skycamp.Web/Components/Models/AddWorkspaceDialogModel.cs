using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class AddWorkspaceDialogModel
{
    [Required]
    [Length(3, 100)]
    public string Name { get; set; } = null!;

    [Required]
    [Length(3, 500)]
    public string Description { get; set; } = null!;
}