using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.JoinAllAccessProject.Shared;

public class JoinAllAccessProjectCommandHandler : CommandHandler<JoinAllAccessProjectCommand>
{
    private readonly ILogger<JoinAllAccessProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public JoinAllAccessProjectCommandHandler(ILogger<JoinAllAccessProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(JoinAllAccessProjectCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        // Check if user is in the workspace
        var workspaceUser = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == user.Id, ct);

        if (workspaceUser == null)
        {
            ThrowError("You must be a member of the workspace to join this project", statusCode: 403);
        }

        // Verify project exists and is all-access
        var project = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId && p.WorkspaceId == command.WorkspaceId, ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        if (!project.IsAllAccess)
        {
            ThrowError("This project is not marked as all-access", statusCode: 400);
        }

        // Check if user is already in the project
        var existingProjectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == user.Id, ct);

        if (existingProjectUser != null)
        {
            ThrowError("You are already a member of this project", statusCode: 400);
        }

        // Add user as Viewer to the project (all-access projects start as Viewer)
        var projectUser = new ProjectUser
        {
            ProjectId = command.ProjectId,
            UserId = user.Id,
            RoleName = "Viewer",
            JoinedUtc = DateTime.UtcNow
        };

        _dbContext.ProjectUsers.Add(projectUser);
        await _dbContext.SaveChangesAsync(ct);
    }
}

public record JoinAllAccessProjectCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserName { get; init; }
}

public class JoinAllAccessProjectCommandValidator : AbstractValidator<JoinAllAccessProjectCommand>
{
    public JoinAllAccessProjectCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}
