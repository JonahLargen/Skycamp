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
        Post("/projectmanagement/projects");
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
            IsAllAccess = r.IsAllAccess
        };
    }
}

public class CreateProjectRequest
{
    public required Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsAllAccess { get; set; }
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
    }
}

public class CreateProjectResponse
{
    public required Guid Id { get; set; }
}