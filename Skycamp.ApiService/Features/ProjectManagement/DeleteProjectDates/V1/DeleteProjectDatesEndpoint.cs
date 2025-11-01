using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.DeleteProjectDates.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteProjectDates.V1;

public class DeleteProjectDatesEndpoint : EndpointWithoutResponseWithCommandMapping<DeleteProjectDatesRequest, DeleteProjectDatesCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/deletedates");
        Version(1);

        Description(b =>
        {
            b.WithName("DeleteProjectDatesV1");
        });

        Summary(s =>
        {
            s.Summary = "Deletes the dates of a project.";
            s.Description = "Deletes the dates of a project in a specific workspace.";
        });
    }

    public override async Task HandleAsync(DeleteProjectDatesRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DeleteProjectDatesCommand MapToCommand(DeleteProjectDatesRequest r)
    {
        return new DeleteProjectDatesCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            DeleteUserName = User.GetRequiredUserName()
        };
    }
}

public class DeleteProjectDatesRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class DeleteProjectDatesRequestValidator : Validator<DeleteProjectDatesRequest>
{
    public DeleteProjectDatesRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}