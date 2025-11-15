using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectActivities.Shared;

public class GetProjectActivitiesCommandHandler : CommandHandler<GetProjectActivitiesCommand, GetProjectActivitiesResult>
{
    private readonly ILogger<GetProjectActivitiesCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetProjectActivitiesCommandHandler(ILogger<GetProjectActivitiesCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<GetProjectActivitiesResult> ExecuteAsync(GetProjectActivitiesCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == project.WorkspaceId && wu.UserId == user.Id, ct);

        if (workspaceUser == null)
        {
            ThrowError("You do not have access to this project", statusCode: 403);
        }

        // Get recent activities for this project
        var activities = new List<GetProjectActivitiesResultActivity>();

        // Get todo activities from OutboxMessages
        var recentMessages = await _dbContext.OutboxMessages
            .Where(m => m.Payload.Contains($"\"projectId\":\"{command.ProjectId}\""))
            .OrderByDescending(m => m.OccurredOnUtc)
            .Take(command.Limit)
            .ToListAsync(ct);

        foreach (var message in recentMessages)
        {
            string description = "";
            string? userName = null;
            string? avatarUrl = null;

            try
            {
                // Parse the payload to get activity description
                var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Payload);
                
                if (message.Type.Contains("TodoCreatedEventV1"))
                {
                    var text = payload?["text"]?.ToString() ?? "a todo";
                    userName = payload?["createUserDisplayName"]?.ToString();
                    var userId = payload?["createUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"created todo: {text}";
                }
                else if (message.Type.Contains("TodoCompletedEventV1"))
                {
                    var text = payload?["text"]?.ToString() ?? "a todo";
                    userName = payload?["completedByUserDisplayName"]?.ToString();
                    var userId = payload?["completedByUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"completed todo: {text}";
                }
                else if (message.Type.Contains("TodoUncompletedEventV1"))
                {
                    var text = payload?["text"]?.ToString() ?? "a todo";
                    userName = payload?["uncompletedByUserDisplayName"]?.ToString();
                    var userId = payload?["uncompletedByUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"uncompleted todo: {text}";
                }
                else if (message.Type.Contains("TodoTextEditedEventV1"))
                {
                    var newText = payload?["newText"]?.ToString() ?? "a todo";
                    userName = payload?["updateUserDisplayName"]?.ToString();
                    var userId = payload?["updateUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"edited todo text to: {newText}";
                }
                else if (message.Type.Contains("TodoDueDateChangedEventV1"))
                {
                    var text = payload?["text"]?.ToString() ?? "a todo";
                    userName = payload?["updateUserDisplayName"]?.ToString();
                    var userId = payload?["updateUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"changed due date for todo: {text}";
                }
                else if (message.Type.Contains("TodoDeletedEventV1"))
                {
                    var text = payload?["text"]?.ToString() ?? "a todo";
                    userName = payload?["deleteUserDisplayName"]?.ToString();
                    var userId = payload?["deleteUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = $"deleted todo: {text}";
                }
                else if (message.Type.Contains("ProjectCreatedEventV1"))
                {
                    userName = payload?["createUserDisplayName"]?.ToString();
                    var userId = payload?["createUserId"]?.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var activityUser = await _userManager.FindByIdAsync(userId);
                        avatarUrl = activityUser?.AvatarUrl;
                    }
                    description = "created the project";
                }

                if (!string.IsNullOrEmpty(description))
                {
                    activities.Add(new GetProjectActivitiesResultActivity
                    {
                        UserName = userName ?? "Unknown User",
                        UserAvatar = avatarUrl,
                        Description = description,
                        Timestamp = message.OccurredOnUtc
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse activity message {MessageId}", message.Id);
            }
        }

        return new GetProjectActivitiesResult
        {
            Activities = activities
        };
    }
}

public record GetProjectActivitiesCommand : ICommand<GetProjectActivitiesResult>
{
    public required Guid ProjectId { get; set; }
    public required string UserName { get; set; }
    public int Limit { get; set; } = 20;
}

public class GetProjectActivitiesCommandValidator : AbstractValidator<GetProjectActivitiesCommand>
{
    public GetProjectActivitiesCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}

public record GetProjectActivitiesResult
{
    public List<GetProjectActivitiesResultActivity> Activities { get; set; } = [];
}

public record GetProjectActivitiesResultActivity
{
    public required string UserName { get; set; }
    public string? UserAvatar { get; set; }
    public required string Description { get; set; }
    public DateTime Timestamp { get; set; }
}
