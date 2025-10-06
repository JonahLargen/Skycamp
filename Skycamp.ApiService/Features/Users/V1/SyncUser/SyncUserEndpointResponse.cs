using Skycamp.ApiService.Common.Logging;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

[Loggable]
public class SyncUserEndpointResponse
{
    public required string UserId { get; init; }
    public required bool Created { get; init; }
    public required List<string> Roles { get; init; }
}