using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissNotification.Shared;

public record DismissNotificationRequest
{
    public Guid NotificationId { get; set; }
    public string CurrentUserId { get; set; } = null!;
}

public record DismissNotificationResponse
{
    public bool Success { get; set; }
}

public class DismissNotificationCommand
{
    private readonly ApplicationDbContext _dbContext;

    public DismissNotificationCommand(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DismissNotificationResponse> ExecuteAsync(DismissNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = request.CurrentUserId;

        var notification = await _dbContext.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == currentUserId, cancellationToken);

        if (notification == null)
        {
            throw new UnauthorizedAccessException("Notification not found or user does not have access");
        }

        notification.IsDismissed = true;
        notification.DismissedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DismissNotificationResponse
        {
            Success = true
        };
    }
}
