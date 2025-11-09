using System.ComponentModel.DataAnnotations;

namespace Skycamp.Web.Components.Models;

public class EditProjectProgressDialogModel
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    [Required]
    public int Progress { get; set; }
}