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

        // Get recent activities from ProjectActivity table
        var activities = await _dbContext.ProjectActivities
            .Where(pa => pa.ProjectId == command.ProjectId)
            .OrderByDescending(pa => pa.OccurredUtc)
            .Take(command.Limit)
            .Select(pa => new GetProjectActivitiesResultActivity
            {
                UserName = pa.UserDisplayName ?? "Unknown User",
                UserAvatar = pa.UserAvatarUrl,
                Description = pa.Message,
                ActivityType = pa.ActivityType,
                Timestamp = pa.OccurredUtc
            })
            .ToListAsync(ct);

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
    public int Limit { get; set; } = 25;
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
    public required string ActivityType { get; set; }
    public DateTime Timestamp { get; set; }
}
