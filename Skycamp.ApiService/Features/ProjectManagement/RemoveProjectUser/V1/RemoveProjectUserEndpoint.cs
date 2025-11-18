using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.RemoveProjectUser.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.RemoveProjectUser.V1;

public class RemoveProjectUserEndpoint : EndpointWithoutResponseWithCommandMapping<RemoveProjectUserRequest, RemoveProjectUserCommand>
{
    public override void Configure()
    {
        Delete("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/users/{UserId}");
        Version(1);

        Description(b =>
        {
            b.WithName("RemoveProjectUserV1");
        });

        Summary(s =>
        {
            s.Summary = "Removes a user from a project.";
            s.Description = "Removes a user from a project. Requires Owner or Admin role. Admins cannot remove Owners.";
        });
    }

    public override async Task HandleAsync(RemoveProjectUserRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override RemoveProjectUserCommand MapToCommand(RemoveProjectUserRequest r)
    {
        return new RemoveProjectUserCommand()
        {
            CurrentUserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            ProjectId = r.ProjectId,
            UserIdToRemove = r.UserId
        };
    }
}

public record RemoveProjectUserRequest
{
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
    public string UserId { get; init; } = null!;
}

public class RemoveProjectUserRequestValidator : Validator<RemoveProjectUserRequest>
{
    public RemoveProjectUserRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
