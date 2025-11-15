using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.Contracts.Events;

namespace Skycamp.ApiService.Features.TodoManagement.DeleteTodo.Shared;

public class DeleteTodoCommandHandler : CommandHandler<DeleteTodoCommand, DeleteTodoResult>
{
    private readonly ILogger<DeleteTodoCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteTodoCommandHandler(ILogger<DeleteTodoCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<DeleteTodoResult> ExecuteAsync(DeleteTodoCommand command, CancellationToken ct = default)
    {
        var deleteUser = await _userManager.FindByNameAsync(command.DeleteUserName);

        if (deleteUser == null)
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
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == todo.Project.WorkspaceId && wu.UserId == deleteUser.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" or "Member" })
        {
            ThrowError("You do not have access to delete todos in this project", statusCode: 403);
        }

        var outboxMessage = OutboxMessage.Create(new TodoDeletedEventV1()
        {
            Id = todo.Id,
            ProjectId = todo.ProjectId,
            ProjectName = todo.Project.Name,
            Text = todo.Text,
            DeleteUserId = deleteUser.Id,
            DeleteUserDisplayName = deleteUser.DisplayName,
            DeletedUtc = DateTime.UtcNow
        });

        _dbContext.Todos.Remove(todo);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new DeleteTodoResult
        {
            Success = true
        };
    }
}

public record DeleteTodoCommand : ICommand<DeleteTodoResult>
{
    public required Guid TodoId { get; set; }
    public required string DeleteUserName { get; set; }
}

public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();

        RuleFor(x => x.DeleteUserName)
            .NotEmpty();
    }
}

public record DeleteTodoResult
{
    public required bool Success { get; set; }
}
