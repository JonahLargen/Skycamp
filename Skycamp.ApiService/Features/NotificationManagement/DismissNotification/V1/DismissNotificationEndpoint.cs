using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.NotificationManagement.DismissNotification.Shared;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissNotification.V1;

public class DismissNotificationEndpoint : EndpointWithCommandMapping<DismissNotificationRequest, DismissNotificationResponse, DismissNotificationCommand, DismissNotificationResult>
{
    public override void Configure()
    {
        Post("/notificationmanagement/notifications/{NotificationId}/dismiss");
        Version(1);

        Description(b =>
        {
            b.WithName("DismissNotificationV1");
        });

        Summary(s =>
        {
            s.Summary = "Dismiss notification";
            s.Description = "Marks a notification as dismissed.";
        });
    }

    public override async Task HandleAsync(DismissNotificationRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DismissNotificationResponse MapFromEntity(DismissNotificationResult e)
    {
        return new DismissNotificationResponse
        {
            Success = e.Success
        };
    }

    public override DismissNotificationCommand MapToCommand(DismissNotificationRequest r)
    {
        return new DismissNotificationCommand
        {
            NotificationId = r.NotificationId,
            UserId = User.GetRequiredUserName()
        };
    }
}

public record DismissNotificationRequest
{
    public Guid NotificationId { get; init; }
}

public class DismissNotificationRequestValidator : Validator<DismissNotificationRequest>
{
    public DismissNotificationRequestValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty();
    }
}

public record DismissNotificationResponse
{
    public bool Success { get; set; }
}
