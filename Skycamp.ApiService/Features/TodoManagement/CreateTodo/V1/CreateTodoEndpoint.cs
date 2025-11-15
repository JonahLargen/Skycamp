using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.TodoManagement.CreateTodo.Shared;

namespace Skycamp.ApiService.Features.TodoManagement.CreateTodo.V1;

public class CreateTodoEndpoint : EndpointWithCommandMapping<CreateTodoRequest, CreateTodoResponse, CreateTodoCommand, CreateTodoResult>
{
    public override void Configure()
    {
        Post("/todomanagement/projects/{ProjectId}/todos");
        Version(1);

        Description(b =>
        {
            b.WithName("CreateTodoV1");
        });

        Summary(s =>
        {
            s.Summary = "Create a new todo";
            s.Description = "Creates a new todo item in the project.";
        });
    }

    public override async Task HandleAsync(CreateTodoRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override CreateTodoResponse MapFromEntity(CreateTodoResult e)
    {
        return new CreateTodoResponse
        {
            Id = e.Id
        };
    }

    public override CreateTodoCommand MapToCommand(CreateTodoRequest r)
    {
        return new CreateTodoCommand
        {
            ProjectId = r.ProjectId,
            Text = r.Text,
            DueDate = r.DueDate,
            PrimaryAssigneeId = r.PrimaryAssigneeId,
            Notes = r.Notes,
            CreateUserName = User.GetRequiredUserName()
        };
    }
}

public class CreateTodoRequest
{
    public Guid ProjectId { get; set; }
    public string Text { get; set; } = null!;
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? Notes { get; set; }
}

public class CreateTodoRequestValidator : Validator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class CreateTodoResponse
{
    public required Guid Id { get; set; }
}
