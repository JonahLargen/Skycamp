using Destructurama.Attributed;
using FastEndpoints;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

public record SyncUserCommand : ICommand<SyncUserResult>
{
    public required string LoginProvider { get; init; }

    public required string ProviderKey { get; init; }

    [LogMasked]
    public string? Email { get; init; }

    public bool EmailVerified { get; init; }

    public string? DisplayName { get; init; }

    public string? AvatarUrl { get; init; }
}