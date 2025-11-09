namespace Skycamp.Web.Components.Models;

public class EditProjectDatesDialogModel
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}