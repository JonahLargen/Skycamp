using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProject.V1;

public class UpdateProjectEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateProjectRequest, UpdateProjectCommand>
{
    public override void Configure()
    {
        Put("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Update an existing project";
            s.Description = "Updates an existing project in the system.";
        });
    }

    public override async Task HandleAsync(UpdateProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateProjectCommand MapToCommand(UpdateProjectRequest r)
    {
        return new UpdateProjectCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            Name = r.Name,
            Description = r.Description,
            UpdateUserName = User.GetRequiredUserName(),
            IsAllAccess = r.IsAllAccess,
            Progress = r.Progress ?? 0,
            ArchivedUtc = r.ArchivedUtc,
            StartDate = r.StartDate,
            EndDate = r.EndDate
        };
    }
}

public class UpdateProjectRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsAllAccess { get; set; }
    public decimal? Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateProjectRequestValidator : Validator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.IsAllAccess)
            .NotNull();

        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 1)
            .When(x => x.Progress.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => new { x.StartDate, x.EndDate })
            .Must(dates => (dates.StartDate.HasValue && dates.EndDate.HasValue) || (!dates.StartDate.HasValue && !dates.EndDate.HasValue))
            .WithMessage("Both StartDate and EndDate must be provided together or both must be null.");
    }
}