using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.Contracts.Events;

namespace Skycamp.ApiService.Features.TodoManagement.ToggleTodoComplete.Shared;

public class ToggleTodoCompleteCommandHandler : CommandHandler<ToggleTodoCompleteCommand, ToggleTodoCompleteResult>
{
    private readonly ILogger<ToggleTodoCompleteCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ToggleTodoCompleteCommandHandler(ILogger<ToggleTodoCompleteCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<ToggleTodoCompleteResult> ExecuteAsync(ToggleTodoCompleteCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var todo = await _dbContext.Todos
            .Include(t => t.Project)
            .ThenInclude(p => p.Workspace)
            .FirstOrDefaultAsync(t => t.Id == command.TodoId, ct);

        if (todo == null)
        {
            ThrowError("Todo does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == todo.Project.WorkspaceId && wu.UserId == user.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" or "Member" })
        {
            ThrowError("You do not have access to update todos in this project", statusCode: 403);
        }

        var now = DateTime.UtcNow;
        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedUtc = todo.IsCompleted ? now : null;
        todo.LastUpdatedUtc = now;

        _dbContext.Todos.Update(todo);

        // Publish appropriate event
        if (todo.IsCompleted)
        {
            var completedEvent = OutboxMessage.Create(new TodoCompletedEventV1()
            {
                Id = todo.Id,
                ProjectId = todo.ProjectId,
                ProjectName = todo.Project.Name,
                Text = todo.Text,
                CompletedByUserId = user.Id,
                CompletedByUserDisplayName = user.DisplayName,
                CompletedUtc = now
            });

            await _dbContext.OutboxMessages.AddAsync(completedEvent, ct);
        }
        else
        {
            var uncompletedEvent = OutboxMessage.Create(new TodoUncompletedEventV1()
            {
                Id = todo.Id,
                ProjectId = todo.ProjectId,
                ProjectName = todo.Project.Name,
                Text = todo.Text,
                UncompletedByUserId = user.Id,
                UncompletedByUserDisplayName = user.DisplayName,
                UncompletedUtc = now
            });

            await _dbContext.OutboxMessages.AddAsync(uncompletedEvent, ct);
        }

        await _dbContext.SaveChangesAsync(ct);

        return new ToggleTodoCompleteResult
        {
            IsCompleted = todo.IsCompleted
        };
    }
}

public record ToggleTodoCompleteCommand : ICommand<ToggleTodoCompleteResult>
{
    public required Guid TodoId { get; set; }
    public required string UserName { get; set; }
}

public class ToggleTodoCompleteCommandValidator : AbstractValidator<ToggleTodoCompleteCommand>
{
    public ToggleTodoCompleteCommandValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record ToggleTodoCompleteResult
{
    public required bool IsCompleted { get; set; }
}
