using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspaces.Shared;

public class GetWorkspacesCommandHandler : CommandHandler<GetWorkspacesCommand, GetWorkspacesResult>
{
    private readonly ILogger<GetWorkspacesCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetWorkspacesCommandHandler(ILogger<GetWorkspacesCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetWorkspacesResult> ExecuteAsync(GetWorkspacesCommand command, CancellationToken ct = default)
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
            .Select(wu => new GetWorkspacesResultItem
            {
                Id = wu.Workspace.Id,
                Name = wu.Workspace.Name,
                Description = wu.Workspace.Description,
                RoleName = wu.RoleName,
                CreateUserId = wu.Workspace.CreateUserId,
                CreateUserDisplayName = wu.Workspace.CreateUser != null ? wu.Workspace.CreateUser.DisplayName ?? wu.Workspace.CreateUser.UserName : null,
                CreatedUtc = wu.Workspace.CreatedUtc,
                LastUpdatedUtc = wu.Workspace.LastUpdatedUtc
            })
            .ToListAsync(cancellationToken: ct);

        return new GetWorkspacesResult
        {
            Workspaces = workspaces
        };
    }
}

public record GetWorkspacesCommand : ICommand<GetWorkspacesResult>
{
    public required string UserName { get; init; }
}

public class GetWorkspacesCommandValidator : AbstractValidator<GetWorkspacesCommand>
{
    public GetWorkspacesCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetWorkspacesResult
{
    public required List<GetWorkspacesResultItem> Workspaces { get; init; }
}

public record GetWorkspacesResultItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}