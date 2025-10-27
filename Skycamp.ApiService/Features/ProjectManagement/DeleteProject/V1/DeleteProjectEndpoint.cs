using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.DeleteProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteProject.V1;

public class DeleteProjectEndpoint : EndpointWithoutResponseWithCommandMapping<DeleteProjectRequest, DeleteProjectCommand>
{
    public override void Configure()
    {
        Delete("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}");
        Version(1);

        Description(b =>
        {
            b.WithName("DeleteProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Delete an existing project";
            s.Description = "Deletes an existing project in the system.";
        });
    }

    public override async Task HandleAsync(DeleteProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DeleteProjectCommand MapToCommand(DeleteProjectRequest r)
    {
        return new DeleteProjectCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            DeleteUserName = User.GetRequiredUserName()
        };
    }
}

public class DeleteProjectRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class DeleteProjectRequestValidator : Validator<DeleteProjectRequest>
{
    public DeleteProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}