namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserResponse
{
    public required string UserId { get; init; }
    public required bool Created { get; init; }
    public required List<string> Roles { get; init; }
}