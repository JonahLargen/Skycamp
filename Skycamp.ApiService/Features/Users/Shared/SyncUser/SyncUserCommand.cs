using FastEndpoints;
using Skycamp.ApiService.Common.Logging;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

[Loggable]
public class SyncUserCommand : ICommand<SyncUserCommandResponse>
{
    public string UserId { get; set; } = null!;
}