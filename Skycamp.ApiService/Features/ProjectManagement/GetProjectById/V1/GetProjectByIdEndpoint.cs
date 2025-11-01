using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetProjectById.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectById.V1;

public class GetProjectByIdEndpoint : EndpointWithCommandMapping<GetProjectByIdRequest, GetProjectByIdResponse, GetProjectByIdCommand, GetProjectByIdResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}");
        Version(1);

        Description(b =>
        {
            b.WithName("GetProjectByIdV1");
        });

        Summary(s =>
        {
            s.Summary = "Gets a project by project ID.";
            s.Description = "Retrieves the details of a specific project within a workspace using the project ID.";
        });
    }

    public override async Task HandleAsync(GetProjectByIdRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override GetProjectByIdCommand MapToCommand(GetProjectByIdRequest r)
    {
        return new GetProjectByIdCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId,
            ProjectId = r.ProjectId
        };
    }

    public override GetProjectByIdResponse MapFromEntity(GetProjectByIdResult e)
    {
        return new GetProjectByIdResponse()
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            RoleName = e.RoleName,
            IsAllAccess = e.IsAllAccess,
            CreateUserId = e.CreateUserId,
            CreateUserDisplayName = e.CreateUserDisplayName,
            CreatedUtc = e.CreatedUtc,
            LastUpdatedUtc = e.LastUpdatedUtc,
            Progress = e.Progress,
            ArchivedUtc = e.ArchivedUtc,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Users = e.Users.Select(u => new GetProjectByIdResponseUser()
            {
                Id = u.Id,
                UserName = u.UserName,
                DisplayName = u.DisplayName,
                RoleName = u.RoleName
            }).ToList()
        };
    }
}

public record GetProjectByIdRequest
{
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
}

public class GetProjectByIdRequestValidator : Validator<GetProjectByIdRequest>
{
    public GetProjectByIdRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}

public record GetProjectByIdResponse
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
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<GetProjectByIdResponseUser> Users { get; init; } = [];
}

public record GetProjectByIdResponseUser
{
    public required string Id { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public required string RoleName { get; init; }
}