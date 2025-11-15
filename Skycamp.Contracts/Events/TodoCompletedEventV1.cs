namespace Skycamp.Contracts.Events;

public class TodoCompletedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public string? CompletedByUserId { get; init; }
    public string? CompletedByUserDisplayName { get; init; }
    public DateTime CompletedUtc { get; init; }
}
