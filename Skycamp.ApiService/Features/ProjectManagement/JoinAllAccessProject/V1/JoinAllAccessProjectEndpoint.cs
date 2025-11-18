using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.JoinAllAccessProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.JoinAllAccessProject.V1;

public class JoinAllAccessProjectEndpoint : EndpointWithoutResponseWithCommandMapping<EmptyRequest, JoinAllAccessProjectCommand>
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

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        await SendMappedAsync(request, ct: ct);
    }

    public override JoinAllAccessProjectCommand MapToCommand(EmptyRequest request)
    {
        return new JoinAllAccessProjectCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = Route<Guid>("WorkspaceId"),
            ProjectId = Route<Guid>("ProjectId")
        };
    }
}