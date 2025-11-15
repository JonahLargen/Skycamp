using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.TodoManagement.DeleteTodo.Shared;

namespace Skycamp.ApiService.Features.TodoManagement.DeleteTodo.V1;

public class DeleteTodoEndpoint : EndpointWithCommandMapping<DeleteTodoRequest, DeleteTodoResponse, DeleteTodoCommand, DeleteTodoResult>
{
    public override void Configure()
    {
        Delete("/todomanagement/todos/{TodoId}");
        Version(1);

        Description(b =>
        {
            b.WithName("DeleteTodoV1");
        });

        Summary(s =>
        {
            s.Summary = "Delete a todo";
            s.Description = "Deletes an existing todo item.";
        });
    }

    public override async Task HandleAsync(DeleteTodoRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override DeleteTodoResponse MapFromEntity(DeleteTodoResult e)
    {
        return new DeleteTodoResponse
        {
            Success = e.Success
        };
    }

    public override DeleteTodoCommand MapToCommand(DeleteTodoRequest r)
    {
        return new DeleteTodoCommand
        {
            TodoId = r.TodoId,
            DeleteUserName = User.GetRequiredUserName()
        };
    }
}

public record DeleteTodoRequest
{
    public Guid TodoId { get; init; }
}

public class DeleteTodoRequestValidator : Validator<DeleteTodoRequest>
{
    public DeleteTodoRequestValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();
    }
}

public class DeleteTodoResponse
{
    public required bool Success { get; set; }
}
