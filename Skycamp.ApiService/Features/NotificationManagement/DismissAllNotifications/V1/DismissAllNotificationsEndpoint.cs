using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.V1;

public class DismissAllNotificationsEndpoint : EndpointWithCommandMapping<DismissAllNotificationsRequest, DismissAllNotificationsResponse, DismissAllNotificationsCommand, DismissAllNotificationsResult>
{
    public override void Configure()
    {
        Post("/notificationmanagement/workspaces/{WorkspaceId}/notifications/dismiss-all");
        Version(1);

        Description(b =>
        {
            b.WithName("DismissAllNotificationsV1");
        });

        Summary(s =>
        {
            s.Summary = "Dismiss all notifications";
            s.Description = "Marks all undismissed notifications in a workspace as dismissed.";
        });
    }

    public override async Task HandleAsync(DismissAllNotificationsRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DismissAllNotificationsResponse MapFromEntity(DismissAllNotificationsResult e)
    {
        return new DismissAllNotificationsResponse
        {
            Success = e.Success,
            Count = e.Count
        };
    }

    public override DismissAllNotificationsCommand MapToCommand(DismissAllNotificationsRequest r)
    {
        return new DismissAllNotificationsCommand
        {
            WorkspaceId = r.WorkspaceId,
            UserId = User.GetRequiredUserName()
        };
    }
}

public record DismissAllNotificationsRequest
{
    public Guid WorkspaceId { get; init; }
}

public class DismissAllNotificationsRequestValidator : Validator<DismissAllNotificationsRequest>
{
    public DismissAllNotificationsRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}

public record DismissAllNotificationsResponse
{
    public bool Success { get; set; }
    public int Count { get; set; }
}
