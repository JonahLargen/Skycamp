using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.BackgroundServices;

public class ProjectActivitySubscriber : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<ProjectActivitySubscriber> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProjectActivitySubscriber(ServiceBusClient client, ILogger<ProjectActivitySubscriber> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _processor = client.CreateProcessor("outbox", "outbox-subscription-activity", new ServiceBusProcessorOptions());
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

        try
        {
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
            
            if (payload == null)
            {
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            string? description = null;
            string? userName = null;
            string? userId = null;
            string? avatarUrl = null;
            string? activityType = null;
            Guid? projectId = null;

            // Parse based on message type
            if (messageType?.Contains("TodoCreatedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("createUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("createUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"created todo: {text}";
                activityType = "TodoCreated";
            }
            else if (messageType?.Contains("TodoCompletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("completedByUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("completedByUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"completed todo: {text}";
                activityType = "TodoCompleted";
            }
            else if (messageType?.Contains("TodoUncompletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("uncompletedByUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("uncompletedByUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"uncompleted todo: {text}";
                activityType = "TodoUncompleted";
            }
            else if (messageType?.Contains("TodoTextEditedEventV1") == true)
            {
                var newText = payload.GetValueOrDefault("newText")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("updateUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("updateUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"edited todo text to: {newText}";
                activityType = "TodoTextEdited";
            }
            else if (messageType?.Contains("TodoDueDateChangedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("updateUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("updateUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"changed due date for todo: {text}";
                activityType = "TodoDueDateChanged";
            }
            else if (messageType?.Contains("TodoDeletedEventV1") == true)
            {
                var text = payload.GetValueOrDefault("text")?.ToString() ?? "a todo";
                userName = payload.GetValueOrDefault("deleteUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("deleteUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("projectId")?.ToString() ?? Guid.Empty.ToString());
                description = $"deleted todo: {text}";
                activityType = "TodoDeleted";
            }
            else if (messageType?.Contains("ProjectCreatedEventV1") == true)
            {
                userName = payload.GetValueOrDefault("createUserDisplayName")?.ToString();
                userId = payload.GetValueOrDefault("createUserId")?.ToString();
                projectId = Guid.Parse(payload.GetValueOrDefault("id")?.ToString() ?? Guid.Empty.ToString());
                description = "created the project";
                activityType = "ProjectCreated";
            }

            // Get user avatar if we have a userId
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                avatarUrl = user?.AvatarUrl;
            }

            // Insert activity record if we have enough information
            if (projectId.HasValue && projectId.Value != Guid.Empty && !string.IsNullOrEmpty(description))
            {
                var activity = new ProjectActivity
                {
                    ProjectId = projectId.Value,
                    ActivityType = activityType ?? "Unknown",
                    Message = description,
                    UserId = userId,
                    UserDisplayName = userName,
                    UserAvatarUrl = avatarUrl,
                    OccurredUtc = DateTime.UtcNow
                };

                await dbContext.ProjectActivities.AddAsync(activity);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing activity message: {MessageType}", messageType);
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message in ProjectActivitySubscriber: {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
