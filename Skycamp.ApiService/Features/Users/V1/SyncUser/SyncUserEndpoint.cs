using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Features.Users.Shared.SyncUser;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserEndpoint : EndpointWithCommandMapping<SyncUserEndpointRequest, SyncUserEndpointResponse, SyncUserCommand, SyncUserCommandResponse>
{
    public SyncUserEndpoint()
    {

    }

    public override void Configure()
    {
        Post("/users/sync");
        Version(1);

        AllowAnonymous();

        Description(b =>
        {
            b.WithName("SyncUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Synchronize user data";
            s.Description = "Synchronizes user data between systems.";
        });
    }

    public override async Task HandleAsync(SyncUserEndpointRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override SyncUserEndpointResponse MapFromEntity(SyncUserCommandResponse e)
    {
        return new SyncUserEndpointResponse
        {
            UserId = e.UserId
        };
    }

    public override SyncUserCommand MapToCommand(SyncUserEndpointRequest r)
    {
        return new SyncUserCommand
        {
            UserId = r.UserId
        };
    }
}