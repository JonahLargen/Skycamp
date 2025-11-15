using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.TodoManagement.GetTodosByProject.Shared;

namespace Skycamp.ApiService.Features.TodoManagement.GetTodosByProject.V1;

public class GetTodosByProjectEndpoint : EndpointWithCommandMapping<GetTodosByProjectRequest, GetTodosByProjectResponse, GetTodosByProjectCommand, GetTodosByProjectResult>
{
    public override void Configure()
    {
        Get("/todomanagement/projects/{ProjectId}/todos");
        Version(1);

        Description(b =>
        {
            b.WithName("GetTodosByProjectV1");
        });

        Summary(s =>
        {
            s.Summary = "Get todos by project";
            s.Description = "Retrieves all todos for a specific project.";
        });
    }

    public override async Task HandleAsync(GetTodosByProjectRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override GetTodosByProjectResponse MapFromEntity(GetTodosByProjectResult e)
    {
        return new GetTodosByProjectResponse
        {
            Todos = e.Todos.Select(t => new GetTodosByProjectResponseTodo
            {
                Id = t.Id,
                Text = t.Text,
                DueDate = t.DueDate,
                PrimaryAssigneeId = t.PrimaryAssigneeId,
                PrimaryAssigneeDisplayName = t.PrimaryAssigneeDisplayName,
                PrimaryAssigneeAvatarUrl = t.PrimaryAssigneeAvatarUrl,
                Notes = t.Notes,
                IsCompleted = t.IsCompleted,
                CompletedUtc = t.CompletedUtc,
                CreateUserId = t.CreateUserId,
                CreateUserDisplayName = t.CreateUserDisplayName,
                CreatedUtc = t.CreatedUtc,
                LastUpdatedUtc = t.LastUpdatedUtc
            }).ToList()
        };
    }

    public override GetTodosByProjectCommand MapToCommand(GetTodosByProjectRequest r)
    {
        return new GetTodosByProjectCommand
        {
            ProjectId = r.ProjectId,
            UserName = User.GetRequiredUserName()
        };
    }
}

public record GetTodosByProjectRequest
{
    public Guid ProjectId { get; init; }
}

public class GetTodosByProjectRequestValidator : Validator<GetTodosByProjectRequest>
{
    public GetTodosByProjectRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}

public record GetTodosByProjectResponse
{
    public List<GetTodosByProjectResponseTodo> Todos { get; set; } = [];
}

public record GetTodosByProjectResponseTodo
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? PrimaryAssigneeDisplayName { get; set; }
    public string? PrimaryAssigneeAvatarUrl { get; set; }
    public string? Notes { get; set; }
    public required bool IsCompleted { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public string? CreateUserId { get; set; }
    public string? CreateUserDisplayName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
