using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateWorkspace.Shared;

public class CreateWorkspaceCommandHandler : CommandHandler<CreateWorkspaceCommand, CreateWorkspaceResult>
{
    private readonly ILogger<CreateWorkspaceCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public CreateWorkspaceCommandHandler(ILogger<CreateWorkspaceCommandHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async override Task<CreateWorkspaceResult> ExecuteAsync(CreateWorkspaceCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Creating new workspace with name {WorkspaceName}", command.Name);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == command.CreateUserId, ct);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var result = await _dbContext.Workspaces.AddAsync(new Workspace
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            Description = command.Description,
            CreateUserId = command.CreateUserId,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        }, ct);

        await _dbContext.WorkspaceUsers.AddAsync(new WorkspaceUser
        {
            WorkspaceId = result.Entity.Id,
            UserId = command.CreateUserId,
            RoleName = "Owner",
            JoinedUtc = DateTime.UtcNow
        }, ct);

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Created new workspace with ID {WorkspaceId}", result.Entity.Id);

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
    public required string CreateUserId { get; set; }
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