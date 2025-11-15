using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.Shared;

public record GetUserNotificationsRequest
{
    public Guid WorkspaceId { get; set; }
    public bool IncludeDismissed { get; set; } = false;
    public string CurrentUserId { get; set; } = null!;
}

public record GetUserNotificationsResponse
{
    public List<UserNotificationDto> Notifications { get; set; } = new();
}

public record UserNotificationDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid? ProjectId { get; set; }
    public string NotificationType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? ActorUserId { get; set; }
    public string? ActorUserDisplayName { get; set; }
    public string? ActorUserAvatarUrl { get; set; }
    public DateTime OccurredUtc { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime? DismissedUtc { get; set; }
}

public class GetUserNotificationsCommand
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserNotificationsCommand(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetUserNotificationsResponse> ExecuteAsync(GetUserNotificationsRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = request.CurrentUserId;

        // Verify user has access to workspace
        var hasAccess = await _dbContext.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == request.WorkspaceId && wu.UserId == currentUserId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this workspace");
        }

        var query = _dbContext.UserNotifications
            .Where(n => n.WorkspaceId == request.WorkspaceId && n.UserId == currentUserId);

        if (!request.IncludeDismissed)
        {
            query = query.Where(n => !n.IsDismissed);
        }

        var notifications = await query
            .OrderByDescending(n => n.OccurredUtc)
            .Take(50) // Limit to most recent 50
            .Select(n => new UserNotificationDto
            {
                Id = n.Id,
                WorkspaceId = n.WorkspaceId,
                ProjectId = n.ProjectId,
                NotificationType = n.NotificationType,
                Title = n.Title,
                Message = n.Message,
                ActorUserId = n.ActorUserId,
                ActorUserDisplayName = n.ActorUserDisplayName,
                ActorUserAvatarUrl = n.ActorUserAvatarUrl,
                OccurredUtc = n.OccurredUtc,
                IsDismissed = n.IsDismissed,
                DismissedUtc = n.DismissedUtc
            })
            .ToListAsync(cancellationToken);

        return new GetUserNotificationsResponse
        {
            Notifications = notifications
        };
    }
}
