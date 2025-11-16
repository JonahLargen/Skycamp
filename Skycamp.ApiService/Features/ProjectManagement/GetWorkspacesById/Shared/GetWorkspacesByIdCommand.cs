using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspacesById.Shared;

public class GetWorkspacesByIdCommandHandler : CommandHandler<GetWorkspacesByIdCommand, GetWorkspacesByIdResult>
{
    private readonly ILogger<GetWorkspacesByIdCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetWorkspacesByIdCommandHandler(ILogger<GetWorkspacesByIdCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetWorkspacesByIdResult> ExecuteAsync(GetWorkspacesByIdCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var workspaces = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .Include(w => w.Workspace)
            .ThenInclude(w => w.CreateUser)
            .Where(wu => wu.UserId == user.Id)
            .OrderBy(wu => wu.Workspace.Name)
            .Select(wu => new GetWorkspacesByIdResultItem
            {
                Id = wu.Workspace.Id,
                Name = wu.Workspace.Name,
                Description = wu.Workspace.Description,
                RoleName = wu.RoleName,
                CreateUserId = wu.Workspace.CreateUserId,
                CreateUserDisplayName = wu.Workspace.CreateUser.DisplayName,
                CreatedUtc = wu.Workspace.CreatedUtc,
                LastUpdatedUtc = wu.Workspace.LastUpdatedUtc,
                MemberCount = _dbContext.WorkspaceUsers.Count(u => u.WorkspaceId == wu.WorkspaceId)
            })
            .ToListAsync(cancellationToken: ct);

        return new GetWorkspacesByIdResult
        {
            Workspaces = workspaces
        };
    }
}

public record GetWorkspacesByIdCommand : ICommand<GetWorkspacesByIdResult>
{
    public required string UserName { get; init; }
}

public class GetWorkspacesByIdCommandValidator : AbstractValidator<GetWorkspacesByIdCommand>
{
    public GetWorkspacesByIdCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetWorkspacesByIdResult
{
    public required List<GetWorkspacesByIdResultItem> Workspaces { get; init; }
}

public record GetWorkspacesByIdResultItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public int MemberCount { get; init; }
}