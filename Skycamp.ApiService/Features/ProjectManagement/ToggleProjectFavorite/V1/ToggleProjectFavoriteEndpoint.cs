using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.ToggleProjectFavorite.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.ToggleProjectFavorite.V1;

public class ToggleProjectFavoriteEndpoint : EndpointWithCommandMapping<ToggleProjectFavoriteRequest, ToggleProjectFavoriteResponse, ToggleProjectFavoriteCommand, ToggleProjectFavoriteResult>
{
    public override void Configure()
    {
        Post("/projectmanagement/workspaces/{WorkspaceId}/projects/{ProjectId}/toggle-favorite");
        Version(1);

        Description(b =>
        {
            b.WithName("ToggleProjectFavoriteV1");
        });

        Summary(s =>
        {
            s.Summary = "Toggles a project as favorite.";
            s.Description = "Toggles a project as favorite or unfavorite for the current user.";
        });
    }

    public override async Task HandleAsync(ToggleProjectFavoriteRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override ToggleProjectFavoriteResponse MapFromEntity(ToggleProjectFavoriteResult e)
    {
        return new ToggleProjectFavoriteResponse
        {
            IsFavorite = e.IsFavorite
        };
    }

    public override ToggleProjectFavoriteCommand MapToCommand(ToggleProjectFavoriteRequest r)
    {
        return new ToggleProjectFavoriteCommand
        {
            ProjectId = r.ProjectId,
            WorkspaceId = r.WorkspaceId,
            UserName = User.GetRequiredUserName(),
        };
    }
}

public class ToggleProjectFavoriteRequest
{
    public Guid ProjectId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class ToggleProjectFavoriteRequestValidator : Validator<ToggleProjectFavoriteRequest>
{
    public ToggleProjectFavoriteRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
           .NotEmpty();
    }
}

public record ToggleProjectFavoriteResponse
{
    public required bool IsFavorite { get; set; }
}
