using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.DeleteProjectDates.Shared;

public class DeleteProjectDatesCommandHandler : CommandHandler<DeleteProjectDatesCommand>
{
    private readonly ILogger<DeleteProjectDatesCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteProjectDatesCommandHandler(ILogger<DeleteProjectDatesCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(DeleteProjectDatesCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.DeleteUserName);

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
            ThrowError("Project is archived", statusCode: 400);
        }

        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == project.Id && pu.UserId == user.Id, ct);

        if (projectUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to update this project", statusCode: 403);
        }

        project.StartDate = null;
        project.EndDate = null;
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
    }
}

public record DeleteProjectDatesCommand : ICommand
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string DeleteUserName { get; set; }
}

public class DeleteProjectDatesCommandValidator : AbstractValidator<DeleteProjectDatesCommand>
{
    public DeleteProjectDatesCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();
    }
}