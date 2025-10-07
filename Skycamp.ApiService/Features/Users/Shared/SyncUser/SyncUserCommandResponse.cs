namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

public record SyncUserCommandResponse
{
    public required string UserId { get; init; }
    public required bool Created { get; init; }
    public required List<string> Roles { get; init; }
}