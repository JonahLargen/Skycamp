using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.Messaging;
using Skycamp.ApiService.Data.ProjectManagement;
using Skycamp.Contracts.Events;

namespace Skycamp.ApiService.Features.TodoManagement.CreateTodo.Shared;

public class CreateTodoCommandHandler : CommandHandler<CreateTodoCommand, CreateTodoResult>
{
    private readonly ILogger<CreateTodoCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateTodoCommandHandler(ILogger<CreateTodoCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<CreateTodoResult> ExecuteAsync(CreateTodoCommand command, CancellationToken ct = default)
    {
        var createUser = await _userManager.FindByNameAsync(command.CreateUserName);

        if (createUser == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var project = await _dbContext.Projects
            .Include(p => p.Workspace)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == project.WorkspaceId && wu.UserId == createUser.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" or "Member" })
        {
            ThrowError("You do not have access to create todos in this project", statusCode: 403);
        }

        ApplicationUser? primaryAssignee = null;
        if (!string.IsNullOrEmpty(command.PrimaryAssigneeId))
        {
            primaryAssignee = await _userManager.FindByIdAsync(command.PrimaryAssigneeId);
        }

        var todoResult = await _dbContext.Todos.AddAsync(new Todo
        {
            ProjectId = command.ProjectId,
            Text = command.Text.Trim(),
            DueDate = command.DueDate,
            PrimaryAssigneeId = command.PrimaryAssigneeId,
            Notes = command.Notes?.Trim(),
            CreateUserId = createUser.Id,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        }, ct);

        var outboxMessage = OutboxMessage.Create(new TodoCreatedEventV1()
        {
            Id = todoResult.Entity.Id,
            ProjectId = todoResult.Entity.ProjectId,
            ProjectName = project.Name,
            Text = todoResult.Entity.Text,
            DueDate = todoResult.Entity.DueDate,
            PrimaryAssigneeId = todoResult.Entity.PrimaryAssigneeId,
            PrimaryAssigneeDisplayName = primaryAssignee?.DisplayName,
            Notes = todoResult.Entity.Notes,
            CreateUserId = createUser.Id,
            CreateUserDisplayName = createUser.DisplayName,
            CreatedUtc = todoResult.Entity.CreatedUtc
        });

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new CreateTodoResult
        {
            Id = todoResult.Entity.Id
        };
    }
}

public record CreateTodoCommand : ICommand<CreateTodoResult>
{
    public required Guid ProjectId { get; set; }
    public required string Text { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? Notes { get; set; }
    public required string CreateUserName { get; set; }
}

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.CreateUserName)
            .NotEmpty();
    }
}

public record CreateTodoResult
{
    public required Guid Id { get; set; }
}
