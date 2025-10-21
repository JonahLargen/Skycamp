using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.CreateWorkspace.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateWorkspace.V1;

public class CreateWorkspaceEndpoint : EndpointWithCommandMapping<CreateWorkspaceRequest, CreateWorkspaceResponse, CreateWorkspaceCommand, CreateWorkspaceResult>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces");
        Version(1);

        Description(b =>
        {
            b.WithName("CreateWorkspaceV1");
        });

        Summary(s =>
        {
            s.Summary = "Create a new workspace";
            s.Description = "Creates a new workspace in the system.";
        });
    }

    public override async Task HandleAsync(CreateWorkspaceRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override CreateWorkspaceResponse MapFromEntity(CreateWorkspaceResult e)
    {
        return new CreateWorkspaceResponse
        {
            Id = e.Id
        };
    }

    public override CreateWorkspaceCommand MapToCommand(CreateWorkspaceRequest r)
    {
        return new CreateWorkspaceCommand
        {
            Name = r.Name,
            Description = r.Description,
            CreateUserName = User.GetRequiredUserName()
        };
    }
}

public record CreateWorkspaceRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class CreateWorkspaceRequestValidator : Validator<CreateWorkspaceRequest>
{
    public CreateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public record CreateWorkspaceResponse
{
    public required Guid Id { get; set; }
}