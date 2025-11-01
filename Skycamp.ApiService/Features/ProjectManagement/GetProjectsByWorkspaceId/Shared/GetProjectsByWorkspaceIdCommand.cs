using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectsByWorkspaceId.Shared;

public class GetProjectsByWorkspaceIdCommandHandler : CommandHandler<GetProjectsByWorkspaceIdCommand, GetProjectsByWorkspaceIdResult>
{
    private readonly ILogger<GetProjectsByWorkspaceIdCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetProjectsByWorkspaceIdCommandHandler(ILogger<GetProjectsByWorkspaceIdCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetProjectsByWorkspaceIdResult> ExecuteAsync(GetProjectsByWorkspaceIdCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var projects = await _dbContext.ProjectUsers
            .AsNoTracking()
            .Include(p => p.Project)
            .ThenInclude(p => p.CreateUser)
            .Where(pu => pu.UserId == user.Id && pu.Project.WorkspaceId == command.WorkspaceId)
            .OrderBy(pu => pu.Project.Name)
            .Select(pu => new GetProjectsByWorkspaceIdResultItem
            {
                Id = pu.Project.Id,
                Name = pu.Project.Name,
                Description = pu.Project.Description,
                RoleName = pu.RoleName,
                IsAllAccess = pu.Project.IsAllAccess,
                CreateUserId = pu.Project.CreateUserId,
                CreateUserDisplayName = pu.Project.CreateUser != null ? pu.Project.CreateUser.DisplayName ?? pu.Project.CreateUser.UserName : null,
                CreatedUtc = pu.Project.CreatedUtc,
                LastUpdatedUtc = pu.Project.LastUpdatedUtc,
                Progress = pu.Project.Progress,
                ArchivedUtc = pu.Project.ArchivedUtc,
                StartDate = pu.Project.StartDate,
                EndDate = pu.Project.EndDate
            })
            .ToListAsync(cancellationToken: ct);

        return new GetProjectsByWorkspaceIdResult
        {
            Projects = projects
        };
    }
}

public record GetProjectsByWorkspaceIdCommand : ICommand<GetProjectsByWorkspaceIdResult>
{
    public required Guid WorkspaceId { get; init; }
    public required string UserName { get; init; }
}

public class GetProjectsByWorkspaceIdCommandValidator : AbstractValidator<GetProjectsByWorkspaceIdCommand>
{
    public GetProjectsByWorkspaceIdCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetProjectsByWorkspaceIdResult
{
    public required List<GetProjectsByWorkspaceIdResultItem> Projects { get; init; }
}

public record GetProjectsByWorkspaceIdResultItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public bool IsAllAccess { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public required decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}