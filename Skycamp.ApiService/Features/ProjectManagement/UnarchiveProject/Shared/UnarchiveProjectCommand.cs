using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UnarchiveProject.Shared;

public class UnarchiveProjectCommandHandler : CommandHandler<UnarchiveProjectCommand, UnarchiveProjectResult>
{
    private readonly ILogger<UnarchiveProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UnarchiveProjectCommandHandler(ILogger<UnarchiveProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<UnarchiveProjectResult> ExecuteAsync(UnarchiveProjectCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UnarchiveUserName);

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

        if (project.ArchivedUtc == null)
        {
            ThrowError("Project is not archived", statusCode: 400);
        }

        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == project.Id && pu.UserId == user.Id, ct);

        if (projectUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to unarchive this project", statusCode: 403);
        }

        project.ArchivedUtc = null;
        project.LastUpdatedUtc = DateTime.UtcNow;

        _dbContext.Projects.Update(project);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            ThrowError("Project was updated by another process. Please retry.", statusCode: 409);
        }

        return new UnarchiveProjectResult
        {
            IsArchived = false
        };
    }
}

public record UnarchiveProjectCommand : ICommand<UnarchiveProjectResult>
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string UnarchiveUserName { get; set; }
}

public class UnarchiveProjectCommandValidator : AbstractValidator<UnarchiveProjectCommand>
{
    public UnarchiveProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UnarchiveUserName)
            .NotEmpty();
    }
}

public class UnarchiveProjectResult
{
    public required bool IsArchived { get; set; }
}
