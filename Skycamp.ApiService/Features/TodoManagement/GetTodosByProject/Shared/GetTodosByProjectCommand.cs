using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.TodoManagement.GetTodosByProject.Shared;

public class GetTodosByProjectCommandHandler : CommandHandler<GetTodosByProjectCommand, GetTodosByProjectResult>
{
    private readonly ILogger<GetTodosByProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetTodosByProjectCommandHandler(ILogger<GetTodosByProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<GetTodosByProjectResult> ExecuteAsync(GetTodosByProjectCommand command, CancellationToken ct = default)
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

        var todos = await _dbContext.Todos
            .Include(t => t.PrimaryAssignee)
            .Include(t => t.CreateUser)
            .Where(t => t.ProjectId == command.ProjectId)
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedUtc)
            .Select(t => new GetTodosByProjectResultTodo
            {
                Id = t.Id,
                Text = t.Text,
                DueDate = t.DueDate,
                PrimaryAssigneeId = t.PrimaryAssigneeId,
                PrimaryAssigneeDisplayName = t.PrimaryAssignee.DisplayName,
                PrimaryAssigneeAvatarUrl = t.PrimaryAssignee.AvatarUrl,
                Notes = t.Notes,
                IsCompleted = t.IsCompleted,
                CompletedUtc = t.CompletedUtc,
                CreateUserId = t.CreateUserId,
                CreateUserDisplayName = t.CreateUser.DisplayName,
                CreatedUtc = t.CreatedUtc,
                LastUpdatedUtc = t.LastUpdatedUtc
            })
            .ToListAsync(ct);

        return new GetTodosByProjectResult
        {
            Todos = todos
        };
    }
}

public record GetTodosByProjectCommand : ICommand<GetTodosByProjectResult>
{
    public required Guid ProjectId { get; set; }
    public required string UserName { get; set; }
}

public class GetTodosByProjectCommandValidator : AbstractValidator<GetTodosByProjectCommand>
{
    public GetTodosByProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetTodosByProjectResult
{
    public List<GetTodosByProjectResultTodo> Todos { get; set; } = [];
}

public record GetTodosByProjectResultTodo
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? PrimaryAssigneeDisplayName { get; set; }
    public string? PrimaryAssigneeAvatarUrl { get; set; }
    public string? Notes { get; set; }
    public required bool IsCompleted { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public string? CreateUserId { get; set; }
    public string? CreateUserDisplayName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
