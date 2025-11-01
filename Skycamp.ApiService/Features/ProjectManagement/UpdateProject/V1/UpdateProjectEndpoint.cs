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
            IsAllAccess = r.IsAllAccess
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
    }
}