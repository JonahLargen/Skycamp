using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.DeleteWorkspace.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteWorkspace.V1;

public class DeleteWorkspaceEndpoint : EndpointWithoutResponseWithCommandMapping<DeleteWorkspaceRequest, DeleteWorkspaceCommand>
{
    public override void Configure()
    {
        Delete("/projectmanagement/workspaces/{Id}");
        Version(1);

        Description(b =>
        {
            b.WithName("DeleteWorkspaceV1");
        });

        Summary(s =>
        {
            s.Summary = "Delete an existing workspace";
            s.Description = "Deletes an existing workspace in the system.";
        });
    }

    public override async Task HandleAsync(DeleteWorkspaceRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DeleteWorkspaceCommand MapToCommand(DeleteWorkspaceRequest r)
    {
        return new DeleteWorkspaceCommand
        {
            Id = r.Id,
            DeleteUserName = User.GetRequiredUserName()
        };
    }
}

public record DeleteWorkspaceRequest
{
    public Guid Id { get; set; }
}

public class DeleteWorkspaceRequestValidator : Validator<DeleteWorkspaceRequest>
{
    public DeleteWorkspaceRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}