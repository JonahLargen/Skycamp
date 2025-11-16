using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateProjectUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProjectUser.V1;

public class UpdateProjectUserEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateProjectUserRequest, UpdateProjectUserCommand>
{
    public override void Configure()
    {
        Put("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/users/{UserId}");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateProjectUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Updates a user's role in a project.";
            s.Description = "Updates a project user's role. Requires Owner or Admin role. Admins cannot modify Owners.";
        });
    }

    public override async Task HandleAsync(UpdateProjectUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override UpdateProjectUserCommand MapToCommand(UpdateProjectUserRequest r)
    {
        return new UpdateProjectUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            ProjectId = r.ProjectId,
            UserIdToUpdate = r.UserId,
            RoleName = r.RoleName
        };
    }
}

public record UpdateProjectUserRequest
{
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
    public string UserId { get; init; } = null!;
    public required string RoleName { get; init; }
}

public class UpdateProjectUserRequestValidator : Validator<UpdateProjectUserRequest>
{
    public UpdateProjectUserRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(role => new[] { "Owner", "Admin", "Member", "Viewer" }.Contains(role))
            .WithMessage("RoleName must be Owner, Admin, Member, or Viewer");
    }
}
