using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.ArchiveProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.ArchiveProject.V1;

public class ArchiveProjectEndpoint : EndpointWithoutResponseWithCommandMapping<ArchiveProjectRequest, UpdateProjectProgressCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/archive");
        Version(1);

        Description(b =>
        {
            b.WithName("ArchiveProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Archives an a project.";
            s.Description = "Archives an existing project in the system.";
        });
    }

    public override async Task HandleAsync(ArchiveProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateProjectProgressCommand MapToCommand(ArchiveProjectRequest r)
    {
        return new UpdateProjectProgressCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            ArchiveUserName = User.GetRequiredUserName(),
        };
    }
}

public class ArchiveProjectRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class ArchiveProjectRequestValidator : Validator<ArchiveProjectRequest>
{
    public ArchiveProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}