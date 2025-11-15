using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.GetProjectActivities.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectActivities.V1;

public class GetProjectActivitiesEndpoint : EndpointWithCommandMapping<GetProjectActivitiesRequest, GetProjectActivitiesResponse, GetProjectActivitiesCommand, GetProjectActivitiesResult>
{
    public override void Configure()
    {
        Get("/projectmanagement/projects/{ProjectId}/activities");
        Version(1);

        Description(b =>
        {
            b.WithName("GetProjectActivitiesV1");
        });

        Summary(s =>
        {
            s.Summary = "Get project activities";
            s.Description = "Retrieves recent activities for a specific project.";
        });
    }

    public override async Task HandleAsync(GetProjectActivitiesRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override GetProjectActivitiesResponse MapFromEntity(GetProjectActivitiesResult e)
    {
        return new GetProjectActivitiesResponse
        {
            Activities = e.Activities.Select(a => new GetProjectActivitiesResponseActivity
            {
                UserName = a.UserName,
                UserAvatar = a.UserAvatar,
                Description = a.Description,
                Timestamp = a.Timestamp
            }).ToList()
        };
    }

    public override GetProjectActivitiesCommand MapToCommand(GetProjectActivitiesRequest r)
    {
        return new GetProjectActivitiesCommand
        {
            ProjectId = r.ProjectId,
            UserName = User.GetRequiredUserName(),
            Limit = r.Limit ?? 20
        };
    }
}

public record GetProjectActivitiesRequest
{
    public Guid ProjectId { get; init; }
    public int? Limit { get; init; }
}

public class GetProjectActivitiesRequestValidator : Validator<GetProjectActivitiesRequest>
{
    public GetProjectActivitiesRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .When(x => x.Limit.HasValue);
    }
}

public record GetProjectActivitiesResponse
{
    public List<GetProjectActivitiesResponseActivity> Activities { get; set; } = [];
}

public record GetProjectActivitiesResponseActivity
{
    public required string UserName { get; set; }
    public string? UserAvatar { get; set; }
    public required string Description { get; set; }
    public DateTime Timestamp { get; set; }
}
