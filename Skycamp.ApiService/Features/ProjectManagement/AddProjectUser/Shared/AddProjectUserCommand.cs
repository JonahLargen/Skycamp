using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.AddProjectUser.Shared;

public class AddProjectUserCommandHandler : CommandHandler<AddProjectUserCommand>
{
    private readonly ILogger<AddProjectUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddProjectUserCommandHandler(ILogger<AddProjectUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(AddProjectUserCommand command, CancellationToken ct = default)
    {
        var currentUser = await _userManager.FindByNameAsync(command.CurrentUserName);

        if (currentUser == null)
        {
            ThrowError("Current user does not exist", statusCode: 500);
        }

        // Check if project exists and belongs to the workspace
        var project = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId && p.WorkspaceId == command.WorkspaceId, ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        // Check if current user has permission (Owner or Admin on the project)
        var currentUserProject = await _dbContext.ProjectUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == currentUser.Id, ct);

        if (currentUserProject is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have permission to add users to this project", statusCode: 403);
        }

        // Verify the user to add is in the workspace
        var workspaceUser = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserIdToAdd, ct);

        if (workspaceUser == null)
        {
            ThrowError("User must be a member of the workspace before being added to a project", statusCode: 400);
        }

        // Check if user is already in the project
        var existingProjectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == command.UserIdToAdd, ct);

        if (existingProjectUser != null)
        {
            ThrowError("User is already a member of this project", statusCode: 400);
        }

        // Validate role
        var validRoles = new[] { "Owner", "Admin", "Member", "Viewer" };
        if (!validRoles.Contains(command.RoleName))
        {
            ThrowError("Invalid role name", statusCode: 400);
        }

        // Create new project user
        var projectUser = new ProjectUser
        {
            ProjectId = command.ProjectId,
            UserId = command.UserIdToAdd,
            RoleName = command.RoleName,
            JoinedUtc = DateTime.UtcNow
        };

        _dbContext.ProjectUsers.Add(projectUser);
        await _dbContext.SaveChangesAsync(ct);
    }
}

public record AddProjectUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserIdToAdd { get; init; }
    public required string RoleName { get; init; }
    public required string CurrentUserName { get; init; }
}

public class AddProjectUserCommandValidator : AbstractValidator<AddProjectUserCommand>
{
    public AddProjectUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserIdToAdd)
            .NotEmpty();

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(role => new[] { "Owner", "Admin", "Member", "Viewer" }.Contains(role))
            .WithMessage("RoleName must be Owner, Admin, Member, or Viewer");

        RuleFor(x => x.CurrentUserName)
            .NotEmpty();
    }
}
