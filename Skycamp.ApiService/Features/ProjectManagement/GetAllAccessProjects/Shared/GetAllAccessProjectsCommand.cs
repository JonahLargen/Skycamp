using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetAllAccessProjects.Shared;

public class GetAllAccessProjectsCommandHandler : CommandHandler<GetAllAccessProjectsCommand, GetAllAccessProjectsResult?>
{
    private readonly ILogger<GetAllAccessProjectsCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllAccessProjectsCommandHandler(ILogger<GetAllAccessProjectsCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetAllAccessProjectsResult?> ExecuteAsync(GetAllAccessProjectsCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        // Check if user is in the workspace
        var workspaceUser = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == user.Id, ct);

        if (workspaceUser == null)
        {
            ThrowError("You do not have access to this workspace", statusCode: 403);
        }

        // Get all all-access projects in this workspace that the user is NOT already a member of
        var allAccessProjects = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.WorkspaceId == command.WorkspaceId && p.IsAllAccess && !p.Users.Any(pu => pu.UserId == user.Id))
            .Select(p => new GetAllAccessProjectsResultProject
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Progress = p.Progress,
                CreatedUtc = p.CreatedUtc,
                ArchivedUtc = p.ArchivedUtc
            })
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return new GetAllAccessProjectsResult
        {
            Projects = allAccessProjects
        };
    }
}

public record GetAllAccessProjectsCommand : ICommand<GetAllAccessProjectsResult>
{
    public required Guid WorkspaceId { get; init; }
    public required string UserName { get; init; }
}

public class GetAllAccessProjectsCommandValidator : AbstractValidator<GetAllAccessProjectsCommand>
{
    public GetAllAccessProjectsCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetAllAccessProjectsResult
{
    public List<GetAllAccessProjectsResultProject> Projects { get; init; } = [];
}

public record GetAllAccessProjectsResultProject
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Progress { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? ArchivedUtc { get; init; }
}
