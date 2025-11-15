using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.GetUserNotifications.Shared;

public class GetUserNotificationsCommandHandler : CommandHandler<GetUserNotificationsCommand, GetUserNotificationsResult>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserNotificationsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetUserNotificationsResult> ExecuteAsync(GetUserNotificationsCommand command, CancellationToken ct = default)
    {
        // Verify user has access to workspace
        var hasAccess = await _dbContext.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserId, ct);

        if (!hasAccess)
        {
            ThrowError("You do not have access to this workspace", statusCode: 403);
        }

        var query = _dbContext.UserNotifications
            .Where(n => n.WorkspaceId == command.WorkspaceId && n.UserId == command.UserId);

        if (!command.IncludeDismissed)
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
            .ToListAsync(ct);

        return new GetUserNotificationsResult
        {
            Notifications = notifications
        };
    }
}

public record GetUserNotificationsCommand : ICommand<GetUserNotificationsResult>
{
    public required Guid WorkspaceId { get; set; }
    public bool IncludeDismissed { get; set; } = false;
    public required string UserId { get; set; }
}

public class GetUserNotificationsCommandValidator : AbstractValidator<GetUserNotificationsCommand>
{
    public GetUserNotificationsCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public record GetUserNotificationsResult
{
    public List<UserNotificationDto> Notifications { get; set; } = [];
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
