using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.CreateProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateProject.V1;

public class CreateProjectEndpoint : EndpointWithCommandMapping<CreateProjectRequest, CreateProjectResponse, CreateProjectCommand, CreateProjectResult>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects");
        Version(1);

        Description(b =>
        {
            b.WithName("CreateProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Create a new project";
            s.Description = "Creates a new project in the system.";
        });
    }

    public override async Task HandleAsync(CreateProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override CreateProjectResponse MapFromEntity(CreateProjectResult e)
    {
        return new CreateProjectResponse
        {
            Id = e.Id
        };
    }

    public override CreateProjectCommand MapToCommand(CreateProjectRequest r)
    {
        return new CreateProjectCommand
        {
            WorkspaceId = r.WorkspaceId,
            Name = r.Name,
            Description = r.Description,
            CreateUserName = User.GetRequiredUserName(),
            IsAllAccess = r.IsAllAccess,
            Progress = r.Progress ?? 0,
            ArchivedUtc = r.ArchivedUtc,
            StartDate = r.StartDate,
            EndDate = r.EndDate
        };
    }
}

public class CreateProjectRequest
{
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsAllAccess { get; set; }
    public decimal? Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateProjectRequestValidator : Validator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
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

public class CreateProjectResponse
{
    public required Guid Id { get; set; }
}