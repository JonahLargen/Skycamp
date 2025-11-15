namespace Skycamp.Contracts.Events;

public class TodoCreatedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public DateOnly? DueDate { get; init; }
    public string? PrimaryAssigneeId { get; init; }
    public string? PrimaryAssigneeDisplayName { get; init; }
    public string? Notes { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
}
