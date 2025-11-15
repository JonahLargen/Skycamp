using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.ArchiveProject.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.ArchiveProject.V1;

public class ArchiveProjectEndpoint : EndpointWithoutRequestWithCommandMapping<ArchiveProjectResponse, UpdateProjectProgressCommand, ArchiveProjectResult>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/archive");
        Version(1);

        Description(b =>
        {
            b.WithName("ArchiveProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Archive a project";
            s.Description = "Archives an existing project in the system.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendMappedAsync(ct: ct);
    }

    public override ArchiveProjectResponse MapFromEntity(ArchiveProjectResult e)
    {
        return new ArchiveProjectResponse
        {
            IsArchived = e.IsArchived
        };
    }

    public override UpdateProjectProgressCommand MapToCommand()
    {
        return new UpdateProjectProgressCommand
        {
            ProjectId = Route<Guid>("ProjectId"),
            WorkspaceId = Route<Guid>("WorkspaceId"),
            ArchiveUserName = User.GetRequiredUserName(),
        };
    }
}

public class ArchiveProjectResponse
{
    public required bool IsArchived { get; set; }
}

public class ArchiveProjectRequestValidator : Validator<UpdateProjectProgressCommand>
{
    public ArchiveProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}