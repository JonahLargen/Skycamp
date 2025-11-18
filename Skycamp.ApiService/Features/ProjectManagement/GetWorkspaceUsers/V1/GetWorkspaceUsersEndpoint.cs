using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetWorkspaceUsers.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspaceUsers.V1;

public class GetWorkspaceUsersEndpoint : EndpointWithCommandMapping<GetWorkspaceUsersRequest, GetWorkspaceUsersResponse, GetWorkspaceUsersCommand, GetWorkspaceUsersResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/workspaces/{WorkspaceId}/users");
        Version(1);

        Description(b =>
        {
            b.WithName("GetWorkspaceUsersV1");
        });

        Summary(s =>
        {
            s.Summary = "Gets all users in a workspace.";
            s.Description = "Retrieves the list of all users and their roles within a workspace.";
        });
    }

    public override async Task HandleAsync(GetWorkspaceUsersRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override GetWorkspaceUsersCommand MapToCommand(GetWorkspaceUsersRequest r)
    {
        return new GetWorkspaceUsersCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId
        };
    }

    public override GetWorkspaceUsersResponse MapFromEntity(GetWorkspaceUsersResult e)
    {
        return new GetWorkspaceUsersResponse()
        {
            Users = e.Users.Select(u => new GetWorkspaceUsersResponseUser()
            {
                UserId = u.UserId,
                UserName = u.UserName,
                DisplayName = u.DisplayName,
                Email = u.Email,
                AvatarUrl = u.AvatarUrl,
                RoleName = u.RoleName,
                JoinedUtc = u.JoinedUtc
            }).ToList()
        };
    }
}

public record GetWorkspaceUsersRequest
{
    public Guid WorkspaceId { get; init; }
}

public class GetWorkspaceUsersRequestValidator : Validator<GetWorkspaceUsersRequest>
{
    public GetWorkspaceUsersRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}

public record GetWorkspaceUsersResponse
{
    public List<GetWorkspaceUsersResponseUser> Users { get; init; } = [];
}

public record GetWorkspaceUsersResponseUser
{
    public required string UserId { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? Email { get; init; }
    public string? AvatarUrl { get; init; }
    public required string RoleName { get; init; }
    public DateTime JoinedUtc { get; init; }
}
