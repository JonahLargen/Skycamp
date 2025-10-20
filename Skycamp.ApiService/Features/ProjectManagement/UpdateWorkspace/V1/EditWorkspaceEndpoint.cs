using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspace.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspace.V1;

public class UpdateWorkspaceEndpoint : EndpointWithoutResponseWithCommandMapping<UpdateWorkspaceRequest, UpdateWorkspaceCommand>
{
    public override void Configure()
    {
        Put("/projectmanagement/workspaces/{Id}");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateWorkspaceV1");
        });

        Summary(s =>
        {
            s.Summary = "Edit an existing workspace";
            s.Description = "Edits an existing workspace in the system.";
        });
    }

    public override async Task HandleAsync(UpdateWorkspaceRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateWorkspaceCommand MapToCommand(UpdateWorkspaceRequest r)
    {
        return new UpdateWorkspaceCommand
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            EditUserName = User.GetRequiredUserName()
        };
    }
}

public record UpdateWorkspaceRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateWorkspaceRequestValidator : Validator<UpdateWorkspaceRequest>
{
    public UpdateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}