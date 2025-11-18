using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.AddWorkspaceUser.Shared;

public class AddWorkspaceUserCommandHandler : CommandHandler<AddWorkspaceUserCommand>
{
    private readonly ILogger<AddWorkspaceUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddWorkspaceUserCommandHandler(ILogger<AddWorkspaceUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(AddWorkspaceUserCommand command, CancellationToken ct = default)
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
            ThrowError("You do not have permission to add users to this workspace", statusCode: 403);
        }

        // Verify the user to add exists
        var userToAdd = await _userManager.FindByIdAsync(command.UserIdToAdd);

        if (userToAdd == null)
        {
            ThrowError("User to add does not exist", statusCode: 404);
        }

        // Check if user is already in the workspace
        var existingWorkspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserIdToAdd, ct);

        if (existingWorkspaceUser != null)
        {
            ThrowError("User is already a member of this workspace", statusCode: 400);
        }

        // Validate role
        var validRoles = new[] { "Owner", "Admin", "Member", "Viewer" };
        if (!validRoles.Contains(command.RoleName))
        {
            ThrowError("Invalid role name", statusCode: 400);
        }

        // Create new workspace user
        var workspaceUser = new WorkspaceUser
        {
            WorkspaceId = command.WorkspaceId,
            UserId = command.UserIdToAdd,
            RoleName = command.RoleName,
            JoinedUtc = DateTime.UtcNow
        };

        _dbContext.WorkspaceUsers.Add(workspaceUser);
        await _dbContext.SaveChangesAsync(ct);
    }
}

public record AddWorkspaceUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required string UserIdToAdd { get; init; }
    public required string RoleName { get; init; }
    public required string CurrentUserName { get; init; }
}

public class AddWorkspaceUserCommandValidator : AbstractValidator<AddWorkspaceUserCommand>
{
    public AddWorkspaceUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
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
