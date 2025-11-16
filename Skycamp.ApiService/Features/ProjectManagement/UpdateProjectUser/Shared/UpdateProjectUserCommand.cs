using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProjectUser.Shared;

public class UpdateProjectUserCommandHandler : CommandHandler<UpdateProjectUserCommand>
{
    private readonly ILogger<UpdateProjectUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProjectUserCommandHandler(ILogger<UpdateProjectUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(UpdateProjectUserCommand command, CancellationToken ct = default)
    {
        var currentUser = await _userManager.FindByNameAsync(command.CurrentUserName);

        if (currentUser == null)
        {
            ThrowError("Current user does not exist", statusCode: 500);
        }

        // Check if current user has permission (Owner or Admin on the project)
        var currentUserProject = await _dbContext.ProjectUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == currentUser.Id, ct);

        if (currentUserProject is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have permission to update users in this project", statusCode: 403);
        }

        // Get the user to update
        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == command.UserIdToUpdate, ct);

        if (projectUser == null)
        {
            ThrowError("User is not a member of this project", statusCode: 404);
        }

        // Admins cannot change Owner roles
        if (currentUserProject.RoleName == "Admin" && projectUser.RoleName == "Owner")
        {
            ThrowError("Admins cannot modify project Owners", statusCode: 403);
        }

        // Admins cannot promote users to Owner
        if (currentUserProject.RoleName == "Admin" && command.RoleName == "Owner")
        {
            ThrowError("Admins cannot promote users to Owner", statusCode: 403);
        }

        // Validate role
        var validRoles = new[] { "Owner", "Admin", "Member", "Viewer" };
        if (!validRoles.Contains(command.RoleName))
        {
            ThrowError("Invalid role name", statusCode: 400);
        }

        projectUser.RoleName = command.RoleName;

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record UpdateProjectUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserIdToUpdate { get; init; }
    public required string RoleName { get; init; }
    public required string CurrentUserName { get; init; }
}

public class UpdateProjectUserCommandValidator : AbstractValidator<UpdateProjectUserCommand>
{
    public UpdateProjectUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
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
