using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.AddProjectUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.AddProjectUser.V1;

public class AddProjectUserEndpoint : EndpointWithoutResponseWithCommandMapping<AddProjectUserRequest, AddProjectUserCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/users");
        Version(1);

        Description(b =>
        {
            b.WithName("AddProjectUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Adds a user to a project.";
            s.Description = "Adds a workspace user to a project with the specified role. Requires Owner or Admin role on the project.";
        });
    }

    public override async Task HandleAsync(AddProjectUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override AddProjectUserCommand MapToCommand(AddProjectUserRequest r)
    {
        return new AddProjectUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            ProjectId = r.ProjectId,
            UserIdToAdd = r.UserId,
            RoleName = r.RoleName
        };
    }
}

public record AddProjectUserRequest
{
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
    public required string UserId { get; init; }
    public required string RoleName { get; init; }
}

public class AddProjectUserRequestValidator : Validator<AddProjectUserRequest>
{
    public AddProjectUserRequestValidator()
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
