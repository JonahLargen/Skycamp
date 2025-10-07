using FastEndpoints;
using Skycamp.ApiService.Common.Logging;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

[Loggable]
public record SyncUserCommand : ICommand<SyncUserCommandResponse>
{
    public required string LoginProvider { get; init; }
    public required string ProviderKey { get; init; }
    public string? Email { get; init; }
    public bool EmailVerified { get; init; }
    public string? DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}