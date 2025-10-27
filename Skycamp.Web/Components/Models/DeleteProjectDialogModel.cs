namespace Skycamp.Web.Components.Models;

public class DeleteProjectDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required Guid ProjectId { get; set; }
}
