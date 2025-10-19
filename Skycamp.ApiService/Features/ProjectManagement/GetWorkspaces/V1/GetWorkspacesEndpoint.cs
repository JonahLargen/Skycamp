using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetWorkspaces.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspaces.V1;

public class GetWorkspacesEndpoint : EndpointWithoutRequestWithCommandMapping<GetWorkspacesResponse, GetWorkspacesCommand, GetWorkspacesResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/workspaces");
        Version(1);

        Description(b =>
        {
            b.WithName("GetWorkspacesV1");
        });

        Summary(s =>
        {
            s.Summary = "Get all workspaces";
            s.Description = "Retrieves all workspaces accessible to the user.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendMappedAsync(ct: ct);
    }

    public override GetWorkspacesResponse MapFromEntity(GetWorkspacesResult e)
    {
        return new GetWorkspacesResponse
        {
            Workspaces = e.Workspaces.Select(w => new GetWorkspacesResponseItem
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                CreateUserId = w.CreateUserId,
                CreateUserDisplayName = w.CreateUserDisplayName,
                CreatedUtc = w.CreatedUtc,
                LastUpdatedUtc = w.LastUpdatedUtc
            }).ToList()
        };
    }

    public override GetWorkspacesCommand MapToCommand()
    {
        return new GetWorkspacesCommand
        {
            UserName = User.GetRequiredUserName()
        };
    }
}

public record GetWorkspacesResponse
{
    public required List<GetWorkspacesResponseItem> Workspaces { get; init; }
}

public record GetWorkspacesResponseItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}