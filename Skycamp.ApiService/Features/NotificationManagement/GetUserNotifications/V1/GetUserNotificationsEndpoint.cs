using FastEndpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.V1;

public class GetUserNotificationsEndpoint : Endpoint<GetUserNotificationsRequest, GetUserNotificationsResponse>
{
    private readonly GetUserNotificationsCommand _command;

    public GetUserNotificationsEndpoint(GetUserNotificationsCommand command)
    {
        _command = command;
    }

    public override void Configure()
    {
        Get("/notificationmanagement/workspaces/{workspaceId}/notifications/v1");
        Version(1);
        Options(x => x.RequireAuthorization());
    }

    public override async Task HandleAsync(GetUserNotificationsRequest req, CancellationToken ct)
    {
        req.CurrentUserId = User.GetRequiredUserName();
        var response = await _command.ExecuteAsync(req, ct);
        await SendAsync(response, cancellation: ct);
    }
}
