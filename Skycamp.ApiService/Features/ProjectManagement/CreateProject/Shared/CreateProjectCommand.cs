using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.ProjectManagement;

namespace Skycamp.ApiService.Features.ProjectManagement.CreateProject.Shared;

public class CreateProjectCommandHandler : CommandHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly ILogger<CreateProjectCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public CreateProjectCommandHandler(ILogger<CreateProjectCommandHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async override Task<CreateProjectResult> ExecuteAsync(CreateProjectCommand command, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == command.CreateUserId, ct);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 500);
        }

        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == command.WorkspaceId, ct);

        if (workspace == null)
        {
            ThrowError("Workspace does not exist, or you do not have access to create projects in this workspace", statusCode: 400);
        }

        var workspaceUser = await _dbContext.WorkspaceUsers
            .FirstOrDefaultAsync(wu => wu.WorkspaceId == command.WorkspaceId && wu.UserId == command.CreateUserId, ct);

        if (workspaceUser is not { RoleName: "Owner" or "Admin" or "Member" })
        {
            ThrowError("Workspace does not exist, or you do not have access to create projects in this workspace", statusCode: 400);
        }

        var projectResult = await _dbContext.Projects.AddAsync(new Project
        {
            Id = Guid.CreateVersion7(),
            WorkspaceId = command.WorkspaceId,
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            CreateUserId = command.CreateUserId,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            IsAllAccess = command.IsAllAccess
        }, ct);

        await _dbContext.ProjectUsers.AddAsync(new ProjectUser
        {
            ProjectId = projectResult.Entity.Id,
            UserId = command.CreateUserId,
            RoleName = "Owner",
            JoinedUtc = DateTime.UtcNow
        }, ct);

        await _dbContext.SaveChangesAsync(ct);

        return new CreateProjectResult
        {
            Id = projectResult.Entity.Id
        };
    }
}

public record CreateProjectCommand : ICommand<CreateProjectResult>
{
    public required Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string CreateUserId { get; set; }
    public required bool IsAllAccess { get; set; }
}

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.CreateUserId)
            .NotEmpty();

        RuleFor(x => x.IsAllAccess)
            .NotNull();
    }
}

public record CreateProjectResult
{
    public required Guid Id { get; set; }
}