using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.RemoveProjectUser.Shared;

public class RemoveProjectUserCommandHandler : CommandHandler<RemoveProjectUserCommand>
{
    private readonly ILogger<RemoveProjectUserCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveProjectUserCommandHandler(ILogger<RemoveProjectUserCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(RemoveProjectUserCommand command, CancellationToken ct = default)
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
            ThrowError("You do not have permission to remove users from this project", statusCode: 403);
        }

        // Get the user to remove
        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == command.ProjectId && pu.UserId == command.UserIdToRemove, ct);

        if (projectUser == null)
        {
            ThrowError("User is not a member of this project", statusCode: 404);
        }

        // Admins cannot remove Owners
        if (currentUserProject.RoleName == "Admin" && projectUser.RoleName == "Owner")
        {
            ThrowError("Admins cannot remove project Owners", statusCode: 403);
        }

        // Remove user from project
        _dbContext.ProjectUsers.Remove(projectUser);

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record RemoveProjectUserCommand : ICommand
{
    public required Guid WorkspaceId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserIdToRemove { get; init; }
    public required string CurrentUserName { get; init; }
}

public class RemoveProjectUserCommandValidator : AbstractValidator<RemoveProjectUserCommand>
{
    public RemoveProjectUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.UserIdToRemove)
            .NotEmpty();

        RuleFor(x => x.CurrentUserName)
            .NotEmpty();
    }
}
