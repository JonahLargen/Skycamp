using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetWorkspacesById.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspacesById.V1;

public class GetWorkspacesByIdEndpoint : EndpointWithoutRequestWithCommandMapping<GetWorkspacesByIdResponse, GetWorkspacesByIdCommand, GetWorkspacesByIdResult>
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

    public override GetWorkspacesByIdResponse MapFromEntity(GetWorkspacesByIdResult e)
    {
        return new GetWorkspacesByIdResponse
        {
            Workspaces = e.Workspaces.Select(w => new GetWorkspacesByIdResponseItem
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                RoleName = w.RoleName,
                CreateUserId = w.CreateUserId,
                CreateUserDisplayName = w.CreateUserDisplayName,
                CreatedUtc = w.CreatedUtc,
                LastUpdatedUtc = w.LastUpdatedUtc,
                MemberCount = w.MemberCount
            }).ToList()
        };
    }

    public override GetWorkspacesByIdCommand MapToCommand()
    {
        return new GetWorkspacesByIdCommand
        {
            UserName = User.GetRequiredUserName()
        };
    }
}

public record GetWorkspacesByIdResponse
{
    public required List<GetWorkspacesByIdResponseItem> Workspaces { get; init; }
}

public record GetWorkspacesByIdResponseItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public int MemberCount { get; init; }
}