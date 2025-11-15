namespace Skycamp.Contracts.Events;

public class TodoDeletedEventV1
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public string? DeleteUserId { get; init; }
    public string? DeleteUserDisplayName { get; init; }
    public DateTime DeletedUtc { get; init; }
}
