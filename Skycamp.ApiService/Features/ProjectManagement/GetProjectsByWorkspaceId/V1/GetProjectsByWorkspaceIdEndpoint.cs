using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetProjectsByWorkspaceId.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectsByWorkspaceId.V1;

public class GetProjectByIdEndpoint : EndpointWithCommandMapping<GetProjectsByWorkspaceIdRequest, GetProjectsByWorkspaceIdResponse, GetProjectByIdCommand, GetProjectsByWorkspaceIdResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/workspaces/{WorkspaceId}/projects");
        Version(1);

        Description(b =>
        {
            b.WithName("GetProjectsByWorkspaceIdV1");
        });

        Summary(s =>
        {
            s.Summary = "Get all projects by workspace ID";
            s.Description = "Retrieves all projects within a specific workspace.";
        });
    }

    public override async Task HandleAsync(GetProjectsByWorkspaceIdRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override GetProjectByIdCommand MapToCommand(GetProjectsByWorkspaceIdRequest r)
    {
        return new GetProjectByIdCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId
        };
    }

    public override GetProjectsByWorkspaceIdResponse MapFromEntity(GetProjectsByWorkspaceIdResult e)
    {
        return new GetProjectsByWorkspaceIdResponse
        {
            Projects = e.Projects.Select(w => new GetProjectsByWorkspaceIdResponseItem
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                RoleName = w.RoleName,
                IsAllAccess = w.IsAllAccess,
                CreateUserId = w.CreateUserId,
                CreateUserDisplayName = w.CreateUserDisplayName,
                CreatedUtc = w.CreatedUtc,
                LastUpdatedUtc = w.LastUpdatedUtc,
                Progress = w.Progress,
                ArchivedUtc = w.ArchivedUtc,
                StartDate = w.StartDate,
                EndDate = w.EndDate
            }).ToList()
        };
    }
}

public record GetProjectsByWorkspaceIdRequest
{
    public Guid WorkspaceId { get; init; }
}

public class GetProjectsByWorkspaceIdRequestValidator : Validator<GetProjectsByWorkspaceIdRequest>
{
    public GetProjectsByWorkspaceIdRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}

public record GetProjectsByWorkspaceIdResponse
{
    public required List<GetProjectsByWorkspaceIdResponseItem> Projects { get; init; }
}

public record GetProjectsByWorkspaceIdResponseItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public required bool IsAllAccess { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public required decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}