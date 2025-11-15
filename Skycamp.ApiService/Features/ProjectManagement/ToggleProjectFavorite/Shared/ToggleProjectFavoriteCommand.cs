using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.ProjectManagement.ToggleProjectFavorite.Shared;

public class ToggleProjectFavoriteCommandHandler : CommandHandler<ToggleProjectFavoriteCommand, ToggleProjectFavoriteResult>
{
    private readonly ILogger<ToggleProjectFavoriteCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ToggleProjectFavoriteCommandHandler(ILogger<ToggleProjectFavoriteCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<ToggleProjectFavoriteResult> ExecuteAsync(ToggleProjectFavoriteCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(command.UserName);

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

        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == project.Id && pu.UserId == user.Id, ct);

        if (projectUser == null)
        {
            ThrowError("You do not have access to this project", statusCode: 403);
        }

        projectUser.IsFavorite = !projectUser.IsFavorite;
        _dbContext.ProjectUsers.Update(projectUser);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            ThrowError("Project was updated by another process. Please retry.", statusCode: 409);
        }

        return new ToggleProjectFavoriteResult
        {
            IsFavorite = projectUser.IsFavorite
        };
    }
}

public record ToggleProjectFavoriteCommand : ICommand<ToggleProjectFavoriteResult>
{
    public required Guid ProjectId { get; set; }
    public required Guid WorkspaceId { get; set; }
    public required string UserName { get; set; }
}

public class ToggleProjectFavoriteCommandValidator : AbstractValidator<ToggleProjectFavoriteCommand>
{
    public ToggleProjectFavoriteCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}

public class ToggleProjectFavoriteResult
{
    public required bool IsFavorite { get; set; }
}
