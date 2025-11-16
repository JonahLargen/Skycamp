namespace Skycamp.Web.Models;

public class ManageProjectPeopleDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required Guid ProjectId { get; set; }
}

public class EditProjectUserRoleDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required Guid ProjectId { get; set; }
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    public required string CurrentRole { get; set; }
}

public class RemoveProjectUserDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required Guid ProjectId { get; set; }
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
}
