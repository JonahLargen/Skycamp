using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.RemoveWorkspaceUser.Shared;

public class RemoveWorkspaceUserCommandHandler : CommandHandler<RemoveWorkspaceUserCommand>
{
    private readonly ILogger<RemoveWorkspaceUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveWorkspaceUserCommandHandler(ILogger<RemoveWorkspaceUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(RemoveWorkspaceUserCommand command, CancellationToken ct = default)
    {
        var currentUser = await _userManager.FindByNameAsync(command.CurrentUserName);

        if (currentUser == null)
        {
            ThrowError("Current user does not exist", statusCode: 500);
        }

        // Check if current user has permission (Owner or Admin)
        var currentUserWorkspace = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == currentUser.Id, ct);

        if (currentUserWorkspace is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have permission to remove users from this workspace", statusCode: 403);
        }

        // Get the user to remove
        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserIdToRemove, ct);

        if (workspaceUser == null)
        {
            ThrowError("User is not a member of this workspace", statusCode: 404);
        }

        // Admins cannot remove Owners
        if (currentUserWorkspace.RoleName == "Admin" && workspaceUser.RoleName == "Owner")
        {
            ThrowError("Admins cannot remove workspace Owners", statusCode: 403);
        }

        // Remove user from all projects in this workspace
        var projectUsers = await _dbContext.ProjectUsers
            .Where(pu => pu.Project.WorkspaceId == command.WorkspaceId && pu.UserId == command.UserIdToRemove)
            .ToListAsync(ct);

        _dbContext.ProjectUsers.RemoveRange(projectUsers);

        // Remove user from workspace
        _dbContext.WorkspaceUsers.Remove(workspaceUser);

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record RemoveWorkspaceUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required string UserIdToRemove { get; init; }
    public required string CurrentUserName { get; init; }
}

public class RemoveWorkspaceUserCommandValidator : AbstractValidator<RemoveWorkspaceUserCommand>
{
    public RemoveWorkspaceUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserIdToRemove)
            .NotEmpty();

        RuleFor(x => x.CurrentUserName)
            .NotEmpty();
    }
}
