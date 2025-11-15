using FastEndpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.DismissNotification.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissNotification.V1;

public class DismissNotificationEndpoint : Endpoint<DismissNotificationRequest, DismissNotificationResponse>
{
    private readonly DismissNotificationCommand _command;

    public DismissNotificationEndpoint(DismissNotificationCommand command)
    {
        _command = command;
    }

    public override void Configure()
    {
        Post("/notificationmanagement/notifications/{notificationId}/dismiss/v1");
        Version(1);
        Options(x => x.RequireAuthorization());
    }

    public override async Task HandleAsync(DismissNotificationRequest req, CancellationToken ct)
    {
        req.CurrentUserId = User.GetRequiredUserName();
        var response = await _command.ExecuteAsync(req, ct);
        await SendAsync(response, cancellation: ct);
    }
}
