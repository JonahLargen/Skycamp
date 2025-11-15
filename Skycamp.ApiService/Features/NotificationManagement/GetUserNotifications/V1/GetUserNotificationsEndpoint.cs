using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.V1;

public class GetUserNotificationsEndpoint : EndpointWithCommandMapping<GetUserNotificationsRequest, GetUserNotificationsResponse, GetUserNotificationsCommand, GetUserNotificationsResult>
{
    public override void Configure()
    {
        Get("/notificationmanagement/workspaces/{WorkspaceId}/notifications");
        Version(1);

        Description(b =>
        {
            b.WithName("GetUserNotificationsV1");
        });

        Summary(s =>
        {
            s.Summary = "Get user notifications";
            s.Description = "Retrieves notifications for the current user in a workspace.";
        });
    }

    public override async Task HandleAsync(GetUserNotificationsRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override GetUserNotificationsResponse MapFromEntity(GetUserNotificationsResult e)
    {
        return new GetUserNotificationsResponse
        {
            Notifications = e.Notifications
        };
    }

    public override GetUserNotificationsCommand MapToCommand(GetUserNotificationsRequest r)
    {
        return new GetUserNotificationsCommand
        {
            WorkspaceId = r.WorkspaceId,
            IncludeDismissed = r.IncludeDismissed,
            UserId = User.GetRequiredUserName()
        };
    }
}

public record GetUserNotificationsRequest
{
    public Guid WorkspaceId { get; init; }
    public bool IncludeDismissed { get; init; } = false;
}

public class GetUserNotificationsRequestValidator : Validator<GetUserNotificationsRequest>
{
    public GetUserNotificationsRequestValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}

public record GetUserNotificationsResponse
{
    public List<UserNotificationDto> Notifications { get; set; } = [];
}
