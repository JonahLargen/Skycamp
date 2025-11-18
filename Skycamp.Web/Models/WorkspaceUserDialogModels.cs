namespace Skycamp.Web.Models;

public class UserProfileDialogModel
{
    public required string UserId { get; set; }
}

public class AddWorkspaceMemberDialogModel
{
    public required Guid WorkspaceId { get; set; }
}

public class EditWorkspaceMemberRoleDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    public required string CurrentRole { get; set; }
}

public class RemoveWorkspaceMemberDialogModel
{
    public required Guid WorkspaceId { get; set; }
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
}
