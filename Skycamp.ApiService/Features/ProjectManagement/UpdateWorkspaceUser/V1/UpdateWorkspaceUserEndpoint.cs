using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspaceUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspaceUser.V1;

public class UpdateWorkspaceUserEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateWorkspaceUserRequest, UpdateWorkspaceUserCommand>
{
    public override void Configure()
    {
        Put("/projectmanagement/workspaces/{WorkspaceId}/users/{UserId}");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateWorkspaceUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Updates a user's role in a workspace.";
            s.Description = "Updates a workspace user's role. Requires Owner or Admin role. Admins cannot modify Owners.";
        });
    }

    public override async Task HandleAsync(UpdateWorkspaceUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override UpdateWorkspaceUserCommand MapToCommand(UpdateWorkspaceUserRequest r)
    {
        return new UpdateWorkspaceUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            UserIdToUpdate = r.UserId,
            RoleName = r.RoleName
        };
    }
}

public record UpdateWorkspaceUserRequest
{
    public Guid WorkspaceId { get; init; }
    public string UserId { get; init; } = null!;
    public required string RoleName { get; init; }
}

public class UpdateWorkspaceUserRequestValidator : Validator<UpdateWorkspaceUserRequest>
{
    public UpdateWorkspaceUserRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(role => new[] { "Owner", "Admin", "Member", "Viewer" }.Contains(role))
            .WithMessage("RoleName must be Owner, Admin, Member, or Viewer");
    }
}
