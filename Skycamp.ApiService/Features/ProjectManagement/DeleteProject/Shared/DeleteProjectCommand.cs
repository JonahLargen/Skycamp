using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteProject.Shared;

public class DeleteProjectCommandHandler : CommandHandler<DeleteProjectCommand>
{
    private readonly ILogger<DeleteProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(DeleteProjectCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.DeleteUserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId && p.WorkspaceId == command.WorkspaceId, ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(wu => wu.ProjectId == project.Id && wu.UserId == user.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to delete this project", statusCode: 403);
        }

        var todos = _dbContext.Todos.Where(t => t.ProjectId == project.Id);

        _dbContext.Todos.RemoveRange(todos);

        var projectActivities = _dbContext.ProjectActivities.Where(pa => pa.ProjectId == project.Id);

        _dbContext.ProjectActivities.RemoveRange(projectActivities);

        var projectUsers = _dbContext.ProjectUsers.Where(pu => pu.ProjectId == project.Id);

        _dbContext.ProjectUsers.RemoveRange(projectUsers);

        _dbContext.Projects.Remove(project);

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record DeleteProjectCommand : ICommand
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string DeleteUserName { get; set; }
}

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.DeleteUserName)
            .NotEmpty();
    }
}