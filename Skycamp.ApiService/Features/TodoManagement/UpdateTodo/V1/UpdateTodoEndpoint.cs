using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.TodoManagement.UpdateTodo.Shared;

namespace Skycamp.ApiService.Features.TodoManagement.UpdateTodo.V1;

public class UpdateTodoEndpoint : EndpointWithCommandMapping<UpdateTodoRequest, UpdateTodoResponse, UpdateTodoCommand, UpdateTodoResult>
{
    public override void Configure()
    {
        Put("/todomanagement/todos/{TodoId}");
        Version(1);

        Description(b =>
        {
            b.WithName("UpdateTodoV1");
        });

        Summary(s =>
        {
            s.Summary = "Update a todo";
            s.Description = "Updates an existing todo item.";
        });
    }

    public override async Task HandleAsync(UpdateTodoRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override UpdateTodoResponse MapFromEntity(UpdateTodoResult e)
    {
        return new UpdateTodoResponse
        {
            Success = e.Success
        };
    }

    public override UpdateTodoCommand MapToCommand(UpdateTodoRequest r)
    {
        return new UpdateTodoCommand
        {
            TodoId = r.TodoId,
            Text = r.Text,
            DueDate = r.DueDate,
            PrimaryAssigneeId = r.PrimaryAssigneeId,
            Notes = r.Notes,
            UpdateUserName = User.GetRequiredUserName()
        };
    }
}

public class UpdateTodoRequest
{
    public Guid TodoId { get; set; }
    public string Text { get; set; } = null!;
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTodoRequestValidator : Validator<UpdateTodoRequest>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class UpdateTodoResponse
{
    public required bool Success { get; set; }
}
