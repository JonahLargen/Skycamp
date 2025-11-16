using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetAllAccessProjects.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetAllAccessProjects.V1;

public class GetAllAccessProjectsEndpoint : EndpointWithCommandMapping<GetAllAccessProjectsRequest, GetAllAccessProjectsResponse, GetAllAccessProjectsCommand, GetAllAccessProjectsResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/workspaces/{WorkspaceId}/all-access-projects");
        Version(1);

        Description(b =>
        {
            b.WithName("GetAllAccessProjectsV1");
        });

        Summary(s =>
        {
            s.Summary = "Gets all-access projects in a workspace that the user can join.";
            s.Description = "Retrieves all projects marked as all-access in the workspace that the user is not already a member of.";
        });
    }

    public override async Task HandleAsync(GetAllAccessProjectsRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override GetAllAccessProjectsCommand MapToCommand(GetAllAccessProjectsRequest r)
    {
        return new GetAllAccessProjectsCommand()
        {
            UserName = User.GetRequiredUserName(),
            WorkspaceId = r.WorkspaceId
        };
    }

    public override GetAllAccessProjectsResponse MapFromEntity(GetAllAccessProjectsResult e)
    {
        return new GetAllAccessProjectsResponse()
        {
            Projects = e.Projects.Select(p => new GetAllAccessProjectsResponseProject()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Progress = p.Progress,
                CreatedUtc = p.CreatedUtc,
                ArchivedUtc = p.ArchivedUtc
            }).ToList()
        };
    }
}

public record GetAllAccessProjectsRequest
{
    public Guid WorkspaceId { get; init; }
}

public class GetAllAccessProjectsRequestValidator : Validator<GetAllAccessProjectsRequest>
{
    public GetAllAccessProjectsRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}

public record GetAllAccessProjectsResponse
{
    public List<GetAllAccessProjectsResponseProject> Projects { get; init; } = [];
}

public record GetAllAccessProjectsResponseProject
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Progress { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? ArchivedUtc { get; init; }
}
