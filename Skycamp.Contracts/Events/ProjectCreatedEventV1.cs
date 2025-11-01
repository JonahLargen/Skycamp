namespace Skycamp.Contracts.Events;

public class ProjectCreatedEventV1
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; } = null!;
    public string? CreateUserId { get; init; } = null!;
    public string? CreateUserDisplayName { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public bool IsAllAccess { get; init; }
    public decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}