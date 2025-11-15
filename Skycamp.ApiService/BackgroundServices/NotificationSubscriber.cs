using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Notifications;
using Skycamp.ApiService.Hubs;

namespace Skycamp.ApiService.BackgroundServices;

public class NotificationSubscriber : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<NotificationSubscriber> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NotificationSubscriber(ServiceBusClient client, ILogger<NotificationSubscriber> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _processor = client.CreateProcessor("outbox", "outbox-subscription-notifications", new ServiceBusProcessorOptions());
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += OnMessageReceived;
        _processor.ProcessErrorAsync += OnError;
        return _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var messageType = args.Message.Subject;

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<FeedHub>>();

        try
        {
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
            
            if (payload == null)
            {
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            string? title = null;
            string? message = null;
            string? notificationType = null;
            string? actorUserId = null;
            string? actorUserDisplayName = null;
            string? actorUserAvatarUrl = null;
            Guid? projectId = null;
            Guid? workspaceId = null;

            // Parse based on message type
            if (messageType?.Contains("TodoCreatedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("createUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("createUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "New Todo Created";
                message = text;
                notificationType = "TodoCreated";
            }
            else if (messageType?.Contains("TodoCompletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("completedByUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("completedByUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "Todo Completed";
                message = text;
                notificationType = "TodoCompleted";
            }
            else if (messageType?.Contains("TodoUncompletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("uncompletedByUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("uncompletedByUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "Todo Reopened";
                message = text;
                notificationType = "TodoUncompleted";
            }
            else if (messageType?.Contains("TodoTextEditedEventV1") == true)
            {
                var newText = payload.GetValueOrDefault("newText")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("updateUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("updateUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "Todo Updated";
                message = newText;
                notificationType = "TodoTextEdited";
            }
            else if (messageType?.Contains("TodoDueDateChangedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("updateUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("updateUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "Todo Due Date Changed";
                message = text;
                notificationType = "TodoDueDateChanged";
            }
            else if (messageType?.Contains("TodoDeletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                actorUserDisplayName = payload.GetValueOrDefault("deleteUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("deleteUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "Todo Deleted";
                message = text;
                notificationType = "TodoDeleted";
            }
            else if (messageType?.Contains("ProjectCreatedEventV1") == true)
            {
                actorUserDisplayName = payload.GetValueOrDefault("createUserDisplayName")?.ToString();
                actorUserId = payload.GetValueOrDefault("createUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("id")?.ToString() ?? Guid.Empty.ToString());
                workspaceId = Guid.Parse(payload.GetValueOrDefault("workspaceId")?.ToString() ?? Guid.Empty.ToString());
                
                title = "New Project Created";
                message = payload.GetValueOrDefault("name")?.ToString() ?? "New project";
                notificationType = "ProjectCreated";
            }

            // Get actor user avatar if we have an actorUserId
            if (!string.IsNullOrEmpty(actorUserId))
            {
                var user = await userManager.FindByIdAsync(actorUserId);
                actorUserAvatarUrl = user?.AvatarUrl;
            }

            // Insert notifications for all project users if we have enough information
            if (projectId.HasValue && projectId.Value != Guid.Empty && workspaceId.HasValue && workspaceId.Value != Guid.Empty && !string.IsNullOrEmpty(message))
            {
                // Get all users in the project (except the actor)
                var projectUsers = await dbContext.ProjectUsers
                    .Where(pu => pu.ProjectId == projectId.Value && pu.UserId != actorUserId)
                    .ToListAsync();

                var notifications = new List<UserNotification>();
                foreach (var projectUser in projectUsers)
                {
                    var notification = new UserNotification
                    {
                        WorkspaceId = workspaceId.Value,
                        ProjectId = projectId.Value,
                        UserId = projectUser.UserId,
                        NotificationType = notificationType ?? "Unknown",
                        Title = title ?? "Notification",
                        Message = message,
                        ActorUserId = actorUserId,
                        ActorUserDisplayName = actorUserDisplayName,
                        ActorUserAvatarUrl = actorUserAvatarUrl,
                        OccurredUtc = DateTime.UtcNow,
                        IsDismissed = false
                    };
                    notifications.Add(notification);
                }

                if (notifications.Any())
                {
                    await dbContext.UserNotifications.AddRangeAsync(notifications);
                    await dbContext.SaveChangesAsync();

                    // Send SignalR notifications to online users
                    foreach (var notification in notifications)
                    {
                        try
                        {
                            await hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", new
                            {
                                notification.Id,
                                notification.WorkspaceId,
                                notification.ProjectId,
                                notification.NotificationType,
                                notification.Title,
                                notification.Message,
                                notification.ActorUserId,
                                notification.ActorUserDisplayName,
                                notification.ActorUserAvatarUrl,
                                notification.OccurredUtc
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to send SignalR notification to user {UserId}", notification.UserId);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification message: {MessageType}", messageType);
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error occurred in NotificationSubscriber");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _processor.StopProcessingAsync(stoppingToken);
        await base.StopAsync(stoppingToken);
    }
}
