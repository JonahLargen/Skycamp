using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.EditWorkspace.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.EditWorkspace.V1;

public class EditWorkspaceEndpoint : EndpointWithoutResponseWithCommandMapping<EditWorkspaceRequest, EditWorkspaceCommand>
{
    public override void Configure()
    {
        Put("/projectmanagement/workspaces/{Id}");
        Version(1);

        Description(b =>
        {
            b.WithName("EditWorkspaceV1");
        });

        Summary(s =>
        {
            s.Summary = "Edit an existing workspace";
            s.Description = "Edits an existing workspace in the system.";
        });
    }

    public override async Task HandleAsync(EditWorkspaceRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override EditWorkspaceCommand MapToCommand(EditWorkspaceRequest r)
    {
        return new EditWorkspaceCommand
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            EditUserName = User.GetRequiredUserName()
        };
    }
}

public record EditWorkspaceRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class EditWorkspaceRequestValidator : Validator<EditWorkspaceRequest>
{
    public EditWorkspaceRequestValidator()
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