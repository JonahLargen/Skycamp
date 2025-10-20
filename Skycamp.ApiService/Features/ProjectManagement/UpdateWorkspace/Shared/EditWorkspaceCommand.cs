using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateWorkspace.Shared;

public class UpdateWorkspaceCommandHandler : CommandHandler<UpdateWorkspaceCommand>
{
    private readonly ILogger<UpdateWorkspaceCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateWorkspaceCommandHandler(ILogger<UpdateWorkspaceCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(UpdateWorkspaceCommand command, CancellationToken ct)
    {
        var editUser = await _userManager.FindByNameAsync(command.EditUserName);

        if (editUser == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var workspace = await _dbContext.Workspaces.FirstOrDefaultAsync(w => w.Id == command.Id, ct);

        if (workspace == null)
        {
            ThrowError("Workspace does not exist, or you do not have access to edit this workspace", statusCode: 400);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.Id && wu.UserId == editUser.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("Workspace does not exist, or you do not have access to edit this workspace", statusCode: 400);
        }

        workspace.Name = command.Name.Trim();
        workspace.Description = command.Description?.Trim();
        workspace.LastUpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record UpdateWorkspaceCommand : ICommand
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string EditUserName { get; set; }
}

public class UpdateWorkspaceCommandValidator : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EditUserName)
            .NotEmpty();
    }
}