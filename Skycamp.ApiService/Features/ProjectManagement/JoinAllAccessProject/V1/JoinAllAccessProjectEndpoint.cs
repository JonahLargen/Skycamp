using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.JoinAllAccessProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.JoinAllAccessProject.V1;

public class JoinAllAccessProjectEndpoint : EndpointWithoutResponseWithCommandMapping<JoinAllAccessProjectRequest, JoinAllAccessProjectCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/join");
        Version(1);

        Description(b =>
        {
            b.WithName("JoinAllAccessProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Joins an all-access project.";
            s.Description = "Adds the current user to an all-access project as a Viewer. User must be a workspace member.";
        });
    }

    public override async Task HandleAsync(JoinAllAccessProjectRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override JoinAllAccessProjectCommand MapToCommand(JoinAllAccessProjectRequest r)
    {
        return new JoinAllAccessProjectCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            ProjectId = r.ProjectId
        };
    }
}

public record JoinAllAccessProjectRequest
{
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
}

public class JoinAllAccessProjectRequestValidator : Validator<JoinAllAccessProjectRequest>
{
    public JoinAllAccessProjectRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
