using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspaceUser.Shared;

public class UpdateWorkspaceUserCommandHandler : CommandHandler<UpdateWorkspaceUserCommand>
{
    private readonly ILogger<UpdateWorkspaceUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateWorkspaceUserCommandHandler(ILogger<UpdateWorkspaceUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(UpdateWorkspaceUserCommand command, CancellationToken ct = default)
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
            ThrowError("You do not have permission to update users in this workspace", statusCode: 403);
        }

        // Get the user to update
        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.UserIdToUpdate, ct);

        if (workspaceUser == null)
        {
            ThrowError("User is not a member of this workspace", statusCode: 404);
        }

        // Admins cannot change Owner roles
        if (currentUserWorkspace.RoleName == "Admin" && workspaceUser.RoleName == "Owner")
        {
            ThrowError("Admins cannot modify workspace Owners", statusCode: 403);
        }

        // Admins cannot promote users to Owner
        if (currentUserWorkspace.RoleName == "Admin" && command.RoleName == "Owner")
        {
            ThrowError("Admins cannot promote users to Owner", statusCode: 403);
        }

        // Validate role
        var validRoles = new[] { "Owner", "Admin", "Member", "Viewer" };
        if (!validRoles.Contains(command.RoleName))
        {
            ThrowError("Invalid role name", statusCode: 400);
        }

        workspaceUser.RoleName = command.RoleName;

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record UpdateWorkspaceUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required string UserIdToUpdate { get; init; }
    public required string RoleName { get; init; }
    public required string CurrentUserName { get; init; }
}

public class UpdateWorkspaceUserCommandValidator : AbstractValidator<UpdateWorkspaceUserCommand>
{
    public UpdateWorkspaceUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserIdToUpdate)
            .NotEmpty();

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(role => new[] { "Owner", "Admin", "Member", "Viewer" }.Contains(role))
            .WithMessage("RoleName must be Owner, Admin, Member, or Viewer");

        RuleFor(x => x.CurrentUserName)
            .NotEmpty();
    }
}
