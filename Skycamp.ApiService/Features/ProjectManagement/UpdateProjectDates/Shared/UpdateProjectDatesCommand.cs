using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProjectDates.Shared;

public class UpdateProjectDatesCommandHandler : CommandHandler<UpdateProjectDatesCommand>
{
    private readonly ILogger<UpdateProjectDatesCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProjectDatesCommandHandler(ILogger<UpdateProjectDatesCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(UpdateProjectDatesCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UpdateUserName);

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

        project.StartDate = command.StartDate;
        project.EndDate = command.EndDate;
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

public record UpdateProjectDatesCommand : ICommand
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string UpdateUserName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class UpdateProjectDatesCommandValidator : AbstractValidator<UpdateProjectDatesCommand>
{
    public UpdateProjectDatesCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("EndDate must be greater than or equal to StartDate.");
    }
}