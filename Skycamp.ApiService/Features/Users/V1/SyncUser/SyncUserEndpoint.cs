using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Features.Users.Shared.SyncUser;

namespace Skycamp.ApiService.Features.Users.V1.SyncUser;

public class SyncUserEndpoint : EndpointWithCommandMapping<SyncUserRequest, SyncUserResponse, SyncUserCommand, SyncUserResult>
{
    public SyncUserEndpoint()
    {

    }

    public override void Configure()
    {
        Post("/users/sync");
        Version(1);

        Description(b =>
        {
            b.ExcludeFromDescription();
            b.WithName("SyncUserV1");
        });

        Roles("Admin");

        Summary(s =>
        {
            s.Summary = "Synchronize user data";
            s.Description = "Synchronizes user data between systems.";
        });
    }

    public override async Task HandleAsync(SyncUserRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override SyncUserResponse MapFromEntity(SyncUserResult e)
    {
        return new SyncUserResponse
        {
            UserId = e.UserId,
            Created = e.Created,
            Roles = e.Roles
        };
    }

    public override SyncUserCommand MapToCommand(SyncUserRequest r)
    {
        return new SyncUserCommand
        {
            LoginProvider = r.LoginProvider,
            ProviderKey = r.ProviderKey,
            Email = r.Email,
            EmailVerified = r.EmailVerified,
            DisplayName = r.DisplayName,
            AvatarUrl = r.AvatarUrl
        };
    }
}