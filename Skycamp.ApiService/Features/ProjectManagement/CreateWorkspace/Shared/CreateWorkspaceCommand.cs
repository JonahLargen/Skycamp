using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateWorkspace.Shared;

public class CreateWorkspaceCommandHandler : CommandHandler<CreateWorkspaceCommand, CreateWorkspaceResult>
{
    private readonly ILogger<CreateWorkspaceCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateWorkspaceCommandHandler(ILogger<CreateWorkspaceCommandHandler> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async override Task<CreateWorkspaceResult> ExecuteAsync(CreateWorkspaceCommand command, CancellationToken ct)
    {
        var createUser = await _userManager.FindByNameAsync(command.CreateUserName);

        if (createUser == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var result = await _dbContext.Workspaces.AddAsync(new Workspace
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            CreateUserId = createUser.Id,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        }, ct);

        await _dbContext.WorkspaceUsers.AddAsync(new WorkspaceUser
        {
            WorkspaceId = result.Entity.Id,
            UserId = createUser.Id,
            RoleName = "Owner",
            JoinedUtc = DateTime.UtcNow
        }, ct);

        await _dbContext.SaveChangesAsync(ct);

        return new CreateWorkspaceResult
        {
            Id = result.Entity.Id
        };
    }
}

public record CreateWorkspaceCommand : ICommand<CreateWorkspaceResult>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string CreateUserName { get; set; }
}

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public record CreateWorkspaceResult
{
    public required Guid Id { get; set; }
}