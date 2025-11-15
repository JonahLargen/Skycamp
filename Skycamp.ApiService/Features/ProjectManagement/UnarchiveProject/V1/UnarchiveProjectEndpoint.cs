using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UnarchiveProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UnarchiveProject.V1;

public class UnarchiveProjectEndpoint : EndpointWithCommandMapping<UnarchiveProjectRequest, UnarchiveProjectResponse, UnarchiveProjectCommand, UnarchiveProjectResult>
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

    public override async Task HandleAsync(UnarchiveProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UnarchiveProjectResponse MapFromEntity(UnarchiveProjectResult e)
    {
        return new UnarchiveProjectResponse
        {
            IsArchived = e.IsArchived
        };
    }

    public override UnarchiveProjectCommand MapToCommand(UnarchiveProjectRequest r)
    {
        return new UnarchiveProjectCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            UnarchiveUserName = User.GetRequiredUserName(),
        };
    }
}

public class UnarchiveProjectRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class UnarchiveProjectRequestValidator : Validator<UnarchiveProjectRequest>
{
    public UnarchiveProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}

public record UnarchiveProjectResponse
{
    public required bool IsArchived { get; set; }
}
