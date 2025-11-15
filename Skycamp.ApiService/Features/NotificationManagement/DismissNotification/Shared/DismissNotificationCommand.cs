using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;

namespace Skycamp.ApiService.Features.NotificationManagement.DismissNotification.Shared;

public class DismissNotificationCommandHandler : CommandHandler<DismissNotificationCommand, DismissNotificationResult>
{
    private readonly ApplicationDbContext _dbContext;

    public DismissNotificationCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<DismissNotificationResult> ExecuteAsync(DismissNotificationCommand command, CancellationToken ct = default)
    {
        var notification = await _dbContext.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == command.NotificationId && n.UserId == command.UserId, ct);

        if (notification == null)
        {
            ThrowError("Notification not found or you do not have access", statusCode: 404);
        }

        notification.IsDismissed = true;
        notification.DismissedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        return new DismissNotificationResult
        {
            Success = true
        };
    }
}

public record DismissNotificationCommand : ICommand<DismissNotificationResult>
{
    public required Guid NotificationId { get; set; }
    public required string UserId { get; set; }
}

public class DismissNotificationCommandValidator : AbstractValidator<DismissNotificationCommand>
{
    public DismissNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public record DismissNotificationResult
{
    public bool Success { get; set; }
}
