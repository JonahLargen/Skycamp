using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateProjectDates.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProjectDates.V1;

public class UpdateProjectDatesEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateProjectDatesRequest, UpdateProjectDatesCommand>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/updatedates");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateProjectDatesV1");
        });

        Summary(s =>
        {
            s.Summary = "Updates the dates of a project.";
            s.Description = "Updates the dates of a project in a specific workspace.";
        });
    }

    public override async Task HandleAsync(UpdateProjectDatesRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateProjectDatesCommand MapToCommand(UpdateProjectDatesRequest r)
    {
        return new UpdateProjectDatesCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            UpdateUserName = User.GetRequiredUserName(),
            StartDate = r.StartDate,
            EndDate = r.EndDate
        };
    }
}

public class UpdateProjectDatesRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class UpdateProjectDatesRequestValidator : Validator<UpdateProjectDatesRequest>
{
    public UpdateProjectDatesRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be greater than or equal to StartDate.");
    }
}