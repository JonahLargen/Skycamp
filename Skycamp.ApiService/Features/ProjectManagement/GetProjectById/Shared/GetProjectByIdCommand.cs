using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetProjectById.Shared;

public class GetProjectByIdCommandHandler : CommandHandler<GetProjectByIdCommand, GetProjectByIdResult?>
{
    private readonly ILogger<GetProjectByIdCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetProjectByIdCommandHandler(ILogger<GetProjectByIdCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetProjectByIdResult?> ExecuteAsync(GetProjectByIdCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var project = await _dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Users)
            .Include(p => p.CreateUser)
            .Where(p => p.WorkspaceId == command.WorkspaceId && p.Id == command.ProjectId)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                RoleName = p.Users
                    .Where(u => u.UserId == user.Id)
                    .Select(u => u.RoleName)
                    .FirstOrDefault(),
                p.IsAllAccess,
                p.CreateUserId,
                CreateUserDisplayName = p.CreateUser != null ? p.CreateUser.DisplayName : null,
                p.CreatedUtc,
                p.LastUpdatedUtc,
                p.Progress,
                p.ArchivedUtc,
                p.StartDate,
                p.EndDate,
                Users = p.Users.Select(u => new
                {
                    u.User.Id,
                    u.User.UserName,
                    u.User.DisplayName,
                    u.RoleName
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (project == null)
        {
            ThrowError("Project does not exist", statusCode: 404);
        }

        if (project.RoleName == null)
        {
            ThrowError("You do not have access to this project", statusCode: 403);
        }

        return new GetProjectByIdResult
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RoleName = project.RoleName,
            IsAllAccess = project.IsAllAccess,
            CreateUserId = project.CreateUserId,
            CreateUserDisplayName = project.CreateUserDisplayName,
            CreatedUtc = project.CreatedUtc,
            LastUpdatedUtc = project.LastUpdatedUtc,
            Progress = project.Progress,
            ArchivedUtc = project.ArchivedUtc,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Users = project.Users.Select(u => new GetProjectByIdResultUser
            {
                Id = u.Id,
                UserName = u.UserName,
                DisplayName = u.DisplayName,
                RoleName = u.RoleName
            }).ToList()
        };
    }
}

public record GetProjectByIdCommand : ICommand<GetProjectByIdResult>
{
    public required Guid WorkspaceId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserName { get; init; }
}

public class GetProjectByIdCommandValidator : AbstractValidator<GetProjectByIdCommand>
{
    public GetProjectByIdCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
           .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetProjectByIdResult
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
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<GetProjectByIdResultUser> Users { get; set; } = new();
}

public record GetProjectByIdResultUser
{
    public required string Id { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public required string RoleName { get; init; }
}