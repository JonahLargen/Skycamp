using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.RemoveWorkspaceUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.RemoveWorkspaceUser.V1;

public class RemoveWorkspaceUserEndpoint : EndpointWithoutResponseWithCommandMapping<RemoveWorkspaceUserRequest, RemoveWorkspaceUserCommand>
{
    public override void Configure()
    {
        Delete("/projectmanagement/workspaces/{WorkspaceId}/users/{UserId}");
        Version(1);

        Description(b =>
        {
            b.WithName("RemoveWorkspaceUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Removes a user from a workspace.";
            s.Description = "Removes a user from a workspace and all associated projects. Requires Owner or Admin role. Admins cannot remove Owners.";
        });
    }

    public override async Task HandleAsync(RemoveWorkspaceUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override RemoveWorkspaceUserCommand MapToCommand(RemoveWorkspaceUserRequest r)
    {
        return new RemoveWorkspaceUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            UserIdToRemove = r.UserId
        };
    }
}

public record RemoveWorkspaceUserRequest
{
    public Guid WorkspaceId { get; init; }
    public string UserId { get; init; } = null!;
}

public class RemoveWorkspaceUserRequestValidator : Validator<RemoveWorkspaceUserRequest>
{
    public RemoveWorkspaceUserRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
