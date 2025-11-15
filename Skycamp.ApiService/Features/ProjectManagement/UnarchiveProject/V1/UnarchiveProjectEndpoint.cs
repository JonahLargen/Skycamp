using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UnarchiveProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UnarchiveProject.V1;

public class UnarchiveProjectEndpoint : EndpointWithoutRequestWithCommandMapping<UnarchiveProjectResponse, UnarchiveProjectCommand, UnarchiveProjectResult>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/unarchive");
        Version(1);

        Description(b =>
        {
            b.WithName("UnarchiveProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Unarchives a project.";
            s.Description = "Unarchives an existing project in the system.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendMappedAsync(ct: ct);
    }

    public override UnarchiveProjectResponse MapFromEntity(UnarchiveProjectResult e)
    {
        return new UnarchiveProjectResponse
        {
            IsArchived = e.IsArchived
        };
    }

    public override UnarchiveProjectCommand MapToCommand()
    {
        return new UnarchiveProjectCommand
        {
            ProjectId = Route<Guid>("ProjectId"),
            WorkspaceId = Route<Guid>("WorkspaceId"),
            UnarchiveUserName = User.GetRequiredUserName(),
        };
    }
}

public record UnarchiveProjectResponse
{
    public required bool IsArchived { get; set; }
}
