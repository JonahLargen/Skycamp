namespace Skycamp.Contracts.Events;

public class TodoDueDateChangedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public DateOnly? OldDueDate { get; init; }
    public DateOnly? NewDueDate { get; init; }
    public string? UpdateUserId { get; init; }
    public string? UpdateUserDisplayName { get; init; }
    public DateTime UpdatedUtc { get; init; }
}
