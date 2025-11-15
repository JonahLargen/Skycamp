using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.Shared;

public record DismissAllNotificationsRequest
{
    public Guid WorkspaceId { get; set; }
    public string CurrentUserId { get; set; } = null!;
}

public record DismissAllNotificationsResponse
{
    public bool Success { get; set; }
    public int Count { get; set; }
}

public class DismissAllNotificationsCommand
{
    private readonly ApplicationDbContext _dbContext;

    public DismissAllNotificationsCommand(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DismissAllNotificationsResponse> ExecuteAsync(DismissAllNotificationsRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = request.CurrentUserId;

        // Verify user has access to workspace
        var hasAccess = await _dbContext.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == request.WorkspaceId && wu.UserId == currentUserId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this workspace");
        }

        var notifications = await _dbContext.UserNotifications
            .Where(n => n.WorkspaceId == request.WorkspaceId && n.UserId == currentUserId && !n.IsDismissed)
            .ToListAsync(cancellationToken);

        var count = notifications.Count;
        var now = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.IsDismissed = true;
            notification.DismissedUtc = now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DismissAllNotificationsResponse
        {
            Success = true,
            Count = count
        };
    }
}
