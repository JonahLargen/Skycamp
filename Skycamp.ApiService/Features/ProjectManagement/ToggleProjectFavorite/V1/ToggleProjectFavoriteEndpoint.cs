using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.ProjectManagement.ToggleProjectFavorite.Shared;

namespace Skycamp.ApiService.Features.ProjectManagement.ToggleProjectFavorite.V1;

public class ToggleProjectFavoriteEndpoint : EndpointWithoutRequestWithCommandMapping<ToggleProjectFavoriteResponse, ToggleProjectFavoriteCommand, ToggleProjectFavoriteResult>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendMappedAsync(ct: ct);
    }

    public override ToggleProjectFavoriteResponse MapFromEntity(ToggleProjectFavoriteResult e)
    {
        return new ToggleProjectFavoriteResponse
        {
            IsFavorite = e.IsFavorite
        };
    }

    public override ToggleProjectFavoriteCommand MapToCommand()
    {
        return new ToggleProjectFavoriteCommand
        {
            ProjectId = Route<Guid>("ProjectId"),
            WorkspaceId = Route<Guid>("WorkspaceId"),
            UserName = User.GetRequiredUserName(),
        };
    }
}

public record ToggleProjectFavoriteResponse
{
    public required bool IsFavorite { get; set; }
}
