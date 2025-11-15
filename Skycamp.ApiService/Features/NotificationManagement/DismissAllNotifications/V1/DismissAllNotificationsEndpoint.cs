using FastEndpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.V1;

public class DismissAllNotificationsEndpoint : Endpoint<DismissAllNotificationsRequest, DismissAllNotificationsResponse>
{
    private readonly DismissAllNotificationsCommand _command;

    public DismissAllNotificationsEndpoint(DismissAllNotificationsCommand command)
    {
        _command = command;
    }

    public override void Configure()
    {
        Post("/notificationmanagement/workspaces/{workspaceId}/notifications/dismiss-all/v1");
        Version(1);
        Options(x => x.RequireAuthorization());
    }

    public override async Task HandleAsync(DismissAllNotificationsRequest req, CancellationToken ct)
    {
        req.CurrentUserId = User.GetRequiredUserName();
        var response = await _command.ExecuteAsync(req, ct);
        await SendAsync(response, cancellation: ct);
    }
}
