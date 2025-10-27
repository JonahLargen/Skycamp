using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteWorkspace.Shared;

public class DeleteWorkspaceCommandHandler : CommandHandler<DeleteWorkspaceCommand>
{
    private readonly ILogger<DeleteWorkspaceCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteWorkspaceCommandHandler(ILogger<DeleteWorkspaceCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task ExecuteAsync(DeleteWorkspaceCommand command, CancellationToken ct = default)
    {
        var editUser = await _userManager.FindByNameAsync(command.DeleteUserName);

        if (editUser == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(w => w.Id == command.Id, ct);

        if (workspace == null)
        {
            ThrowError("Workspace does not exist", statusCode: 404);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.Id && wu.UserId == editUser.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to delete this workspace", statusCode: 403);
        }

        _dbContext.Workspaces.Remove(workspace);
        await _dbContext.SaveChangesAsync(ct);
    }
}

public record DeleteWorkspaceCommand : ICommand
{
    public required Guid Id { get; set; }
    public required string DeleteUserName { get; set; }
}

public class DeleteWorkspaceCommandValidator : AbstractValidator<DeleteWorkspaceCommand>
{
    public DeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DeleteUserName)
            .NotEmpty();
    }
}