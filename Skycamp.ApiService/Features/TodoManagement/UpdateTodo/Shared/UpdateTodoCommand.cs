using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.Contracts.Events;

namespace Skycamp.ApiService.Features.TodoManagement.UpdateTodo.Shared;

public class UpdateTodoCommandHandler : CommandHandler<UpdateTodoCommand, UpdateTodoResult>
{
    private readonly ILogger<UpdateTodoCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateTodoCommandHandler(ILogger<UpdateTodoCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<UpdateTodoResult> ExecuteAsync(UpdateTodoCommand command, CancellationToken ct = default)
    {
        var updateUser = await _userManager.FindByNameAsync(command.UpdateUserName);

        if (updateUser == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var todo = await _dbContext.Todos
            .Include(t => t.Project)
            .ThenInclude(p => p.Workspace)
            .Include(t => t.PrimaryAssignee)
            .FirstOrDefaultAsync(t => t.Id == command.TodoId, ct);

        if (todo == null)
        {
            ThrowError("Todo does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == todo.Project.WorkspaceId && wu.UserId == updateUser.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" or "Member" })
        {
            ThrowError("You do not have access to update todos in this project", statusCode: 403);
        }

        var oldText = todo.Text;
        var oldDueDate = todo.DueDate;

        todo.Text = command.Text.Trim();
        todo.DueDate = command.DueDate;
        todo.PrimaryAssigneeId = command.PrimaryAssigneeId;
        todo.Notes = command.Notes?.Trim();
        todo.LastUpdatedUtc = DateTime.UtcNow;

        _dbContext.Todos.Update(todo);

        // Publish events for changes
        if (oldText != todo.Text)
        {
            var textEditedEvent = OutboxMessage.Create(new TodoTextEditedEventV1()
            {
                Id = todo.Id,
                ProjectId = todo.ProjectId,
                ProjectName = todo.Project.Name,
                OldText = oldText,
                NewText = todo.Text,
                UpdateUserId = updateUser.Id,
                UpdateUserDisplayName = updateUser.DisplayName,
                UpdatedUtc = todo.LastUpdatedUtc
            });

            await _dbContext.OutboxMessages.AddAsync(textEditedEvent, ct);
        }

        if (oldDueDate != todo.DueDate)
        {
            ApplicationUser? primaryAssignee = null;
            if (!string.IsNullOrEmpty(todo.PrimaryAssigneeId))
            {
                primaryAssignee = await _userManager.FindByIdAsync(todo.PrimaryAssigneeId);
            }

            var dueDateChangedEvent = OutboxMessage.Create(new TodoDueDateChangedEventV1()
            {
                Id = todo.Id,
                ProjectId = todo.ProjectId,
                ProjectName = todo.Project.Name,
                Text = todo.Text,
                OldDueDate = oldDueDate,
                NewDueDate = todo.DueDate,
                UpdateUserId = updateUser.Id,
                UpdateUserDisplayName = updateUser.DisplayName,
                UpdatedUtc = todo.LastUpdatedUtc
            });

            await _dbContext.OutboxMessages.AddAsync(dueDateChangedEvent, ct);
        }

        await _dbContext.SaveChangesAsync(ct);

        return new UpdateTodoResult
        {
            Success = true
        };
    }
}

public record UpdateTodoCommand : ICommand<UpdateTodoResult>
{
    public required Guid TodoId { get; set; }
    public required string Text { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? Notes { get; set; }
    public required string UpdateUserName { get; set; }
}

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.UpdateUserName)
            .NotEmpty();
    }
}

public record UpdateTodoResult
{
    public required bool Success { get; set; }
}
