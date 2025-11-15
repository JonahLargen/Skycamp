using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissAllNotifications.Shared;

public class DismissAllNotificationsCommandHandler : CommandHandler<DismissAllNotificationsCommand, DismissAllNotificationsResult>
{
    private readonly ApplicationDbContext _dbContext;

    public DismissAllNotificationsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<DismissAllNotificationsResult> ExecuteAsync(DismissAllNotificationsCommand command, CancellationToken ct = default)
    {
        // Verify user has access to workspace
        var hasAccess = await _dbContext.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserId, ct);

        if (!hasAccess)
        {
            ThrowError("You do not have access to this workspace", statusCode: 403);
        }

        var notifications = await _dbContext.UserNotifications
            .Where(n => n.WorkspaceId == command.WorkspaceId && n.UserId == command.UserId && !n.IsDismissed)
            .ToListAsync(ct);

        var count = notifications.Count;
        var now = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.IsDismissed = true;
            notification.DismissedUtc = now;
        }

        await _dbContext.SaveChangesAsync(ct);

        return new DismissAllNotificationsResult
        {
            Success = true,
            Count = count
        };
    }
}

public record DismissAllNotificationsCommand : ICommand<DismissAllNotificationsResult>
{
    public required Guid WorkspaceId { get; set; }
    public required string UserId { get; set; }
}

public class DismissAllNotificationsCommandValidator : AbstractValidator<DismissAllNotificationsCommand>
{
    public DismissAllNotificationsCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public record DismissAllNotificationsResult
{
    public bool Success { get; set; }
    public int Count { get; set; }
}
