using Destructurama.Attributed;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserRequest
{
    public required string LoginProvider { get; init; }

    public required string ProviderKey { get; init; }

    [LogMasked]
    public string? Email { get; init; }

    public bool EmailVerified { get; init; }

    public string? DisplayName { get; init; }

    public string? AvatarUrl { get; init; }
}