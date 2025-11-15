using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.ArchiveProject.Shared;

public class ArchiveProjectCommandHandler : CommandHandler<UpdateProjectProgressCommand, ArchiveProjectResult>
{
    private readonly ILogger<ArchiveProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ArchiveProjectCommandHandler(ILogger<ArchiveProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<ArchiveProjectResult> ExecuteAsync(UpdateProjectProgressCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.ArchiveUserName);

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

        if (project.ArchivedUtc != null)
        {
            ThrowError("Project is already archived", statusCode: 400);
        }

        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == project.Id && pu.UserId == user.Id, ct);

        if (projectUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to archive this project", statusCode: 403);
        }

        project.ArchivedUtc = DateTime.UtcNow;
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

        return new ArchiveProjectResult
        {
            IsArchived = true
        };
    }
}

public record UpdateProjectProgressCommand : ICommand<ArchiveProjectResult>
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string ArchiveUserName { get; set; }
}

public class ArchiveProjectCommandValidator : AbstractValidator<UpdateProjectProgressCommand>
{
    public ArchiveProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ArchiveUserName)
            .NotEmpty();
    }
}

public class ArchiveProjectResult
{
    public required bool IsArchived { get; set; }
}