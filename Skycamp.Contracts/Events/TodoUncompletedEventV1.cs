namespace Skycamp.Contracts.Events;

public class TodoUncompletedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public string? UncompletedByUserId { get; init; }
    public string? UncompletedByUserDisplayName { get; init; }
    public DateTime UncompletedUtc { get; init; }
}
