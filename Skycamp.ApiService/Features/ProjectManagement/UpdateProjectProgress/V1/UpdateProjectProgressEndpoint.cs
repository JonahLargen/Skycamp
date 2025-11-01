using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateProjectProgress.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProjectProgress.V1;

public class UpdateProjectProgressEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateProjectProgressRequest, UpdateProjectProgressCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/updateprogress");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateProjectProgressV1");
        });

        Summary(s =>
        {
            s.Summary = "Updates the progres of a project.";
            s.Description = "Updates the progress of a project in a specific workspace.";
        });
    }

    public override async Task HandleAsync(UpdateProjectProgressRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateProjectProgressCommand MapToCommand(UpdateProjectProgressRequest r)
    {
        return new UpdateProjectProgressCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            UpdateUserName = User.GetRequiredUserName(),
            Progress = r.Progress
        };
    }
}

public class UpdateProjectProgressRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
    public decimal Progress { get; set; }
}

public class UpdateProjectProgressRequestValidator : Validator<UpdateProjectProgressRequest>
{
    public UpdateProjectProgressRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();

        RuleFor(x => x.Progress)
            .NotNull()
            .InclusiveBetween(0, 1);
    }
}