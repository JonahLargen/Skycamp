using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.AddWorkspaceUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.AddWorkspaceUser.V1;

public class AddWorkspaceUserEndpoint : EndpointWithoutResponseWithCommandMapping<AddWorkspaceUserRequest, AddWorkspaceUserCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/users");
        Version(1);

        Description(b =>
        {
            b.WithName("AddWorkspaceUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Adds a user to a workspace.";
            s.Description = "Adds a user to a workspace with the specified role. Requires Owner or Admin role.";
        });
    }

    public override async Task HandleAsync(AddWorkspaceUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override AddWorkspaceUserCommand MapToCommand(AddWorkspaceUserRequest r)
    {
        return new AddWorkspaceUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            UserIdToAdd = r.UserId,
            RoleName = r.RoleName
        };
    }
}

public record AddWorkspaceUserRequest
{
    public Guid WorkspaceId { get; init; }
    public required string UserId { get; init; }
    public required string RoleName { get; init; }
}

public class AddWorkspaceUserRequestValidator : Validator<AddWorkspaceUserRequest>
{
    public AddWorkspaceUserRequestValidator()
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
