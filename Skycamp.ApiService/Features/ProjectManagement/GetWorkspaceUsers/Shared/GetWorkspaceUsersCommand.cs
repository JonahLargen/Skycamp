using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.GetWorkspaceUsers.Shared;

public class GetWorkspaceUsersCommandHandler : CommandHandler<GetWorkspaceUsersCommand, GetWorkspaceUsersResult?>
{
    private readonly ILogger<GetWorkspaceUsersCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetWorkspaceUsersCommandHandler(ILogger<GetWorkspaceUsersCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public override async Task<GetWorkspaceUsersResult?> ExecuteAsync(GetWorkspaceUsersCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        // Check if user has access to this workspace
        var userWorkspace = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == user.Id, ct);

        if (userWorkspace == null)
        {
            ThrowError("You do not have access to this workspace", statusCode: 403);
        }

        var workspaceUsers = await _dbContext.WorkspaceUsers
            .AsNoTracking()
            .Where(wu => wu.WorkspaceId == command.WorkspaceId)
            .Select(wu => new GetWorkspaceUsersResultUser
            {
                UserId = wu.UserId,
                UserName = wu.User.UserName,
                DisplayName = wu.User.DisplayName,
                Email = wu.User.Email,
                AvatarUrl = wu.User.AvatarUrl,
                RoleName = wu.RoleName,
                JoinedUtc = wu.JoinedUtc
            })
            .OrderBy(u => u.RoleName)
            .ThenBy(u => u.DisplayName)
            .ToListAsync(ct);

        return new GetWorkspaceUsersResult
        {
            Users = workspaceUsers
        };
    }
}

public record GetWorkspaceUsersCommand : ICommand<GetWorkspaceUsersResult>
{
    public required Guid WorkspaceId { get; init; }
    public required string UserName { get; init; }
}

public class GetWorkspaceUsersCommandValidator : AbstractValidator<GetWorkspaceUsersCommand>
{
    public GetWorkspaceUsersCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public record GetWorkspaceUsersResult
{
    public List<GetWorkspaceUsersResultUser> Users { get; init; } = [];
}

public record GetWorkspaceUsersResultUser
{
    public required string UserId { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? Email { get; init; }
    public string? AvatarUrl { get; init; }
    public required string RoleName { get; init; }
    public DateTime JoinedUtc { get; init; }
}
