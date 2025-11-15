namespace Skycamp.Contracts.Events;

public class TodoTextEditedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string OldText { get; init; } = null!;
    public string NewText { get; init; } = null!;
    public string? UpdateUserId { get; init; }
    public string? UpdateUserDisplayName { get; init; }
    public DateTime UpdatedUtc { get; init; }
}
