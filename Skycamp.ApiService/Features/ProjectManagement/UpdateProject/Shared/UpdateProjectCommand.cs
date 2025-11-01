using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.UpdateProject.Shared;

public class UpdateProjectCommandHandler : CommandHandler<UpdateProjectCommand>
{
    private readonly ILogger<UpdateProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProjectCommandHandler(ILogger<UpdateProjectCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task ExecuteAsync(UpdateProjectCommand command, CancellationToken ct = default)
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

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == project.WorkspaceId && wu.UserId == user.Id, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" })
        {
            ThrowError("You do not have access to update this project", statusCode: 403);
        }

        project.Name = command.Name.Trim();
        project.Description = command.Description?.Trim();
        project.LastUpdatedUtc = DateTime.UtcNow;
        project.IsAllAccess = command.IsAllAccess;
        project.Progress = command.Progress;
        project.ArchivedUtc = command.ArchivedUtc;
        project.StartDate = command.StartDate;
        project.EndDate = command.EndDate;

        _dbContext.Projects.Update(project);

        await _dbContext.SaveChangesAsync(ct);
    }
}

public record UpdateProjectCommand : ICommand
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string UpdateUserName { get; set; }
    public required bool IsAllAccess { get; set; }
    public required decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UpdateUserName)
            .NotEmpty();

        RuleFor(x => x.IsAllAccess)
            .NotNull();

        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 1);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => new { x.StartDate, x.EndDate })
           .Must(dates => (dates.StartDate.HasValue && dates.EndDate.HasValue) || (!dates.StartDate.HasValue && !dates.EndDate.HasValue))
           .WithMessage("Both StartDate and EndDate must be provided together or both must be null.");
    }
}