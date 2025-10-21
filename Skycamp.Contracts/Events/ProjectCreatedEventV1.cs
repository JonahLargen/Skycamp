namespace Skycamp.Contracts.Events;

public class ProjectCreatedEventV1 : EventBase<Guid>
{
    public override int Version => 1;
    public override Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; } = null!;
    public string? CreateUserId { get; init; } = null!;
    public string? CreateUserDisplayName { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public bool IsAllAccess { get; init; }
    public List<User> Users { get; init; } = [];

    public class User
    {
        public string UserId { get; init; } = null!;
        public string? UserDisplayName { get; init; } = null!;
        public string RoleName { get; init; } = null!;
        public DateTime JoinedUtc { get; init; }
    }
}