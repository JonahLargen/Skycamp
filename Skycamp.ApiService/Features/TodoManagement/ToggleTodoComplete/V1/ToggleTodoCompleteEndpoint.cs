using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Common.Security;
using Skycamp.ApiService.Features.TodoManagement.ToggleTodoComplete.Shared;

namespace Skycamp.ApiService.Features.TodoManagement.ToggleTodoComplete.V1;

public class ToggleTodoCompleteEndpoint : EndpointWithCommandMapping<ToggleTodoCompleteRequest, ToggleTodoCompleteResponse, ToggleTodoCompleteCommand, ToggleTodoCompleteResult>
{
    public override void Configure()
    {
        Post("/todomanagement/todos/{TodoId}/toggle-complete");
        Version(1);

        Description(b =>
        {
            b.WithName("ToggleTodoCompleteV1");
        });

        Summary(s =>
        {
            s.Summary = "Toggle todo completion";
            s.Description = "Toggles the completion status of a todo item.";
        });
    }

    public override async Task HandleAsync(ToggleTodoCompleteRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override ToggleTodoCompleteResponse MapFromEntity(ToggleTodoCompleteResult e)
    {
        return new ToggleTodoCompleteResponse
        {
            IsCompleted = e.IsCompleted
        };
    }

    public override ToggleTodoCompleteCommand MapToCommand(ToggleTodoCompleteRequest r)
    {
        return new ToggleTodoCompleteCommand
        {
            TodoId = r.TodoId,
            UserName = User.GetRequiredUserName()
        };
    }
}

public record ToggleTodoCompleteRequest
{
    public Guid TodoId { get; init; }
}

public class ToggleTodoCompleteRequestValidator : Validator<ToggleTodoCompleteRequest>
{
    public ToggleTodoCompleteRequestValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty();
    }
}

public class ToggleTodoCompleteResponse
{
    public required bool IsCompleted { get; set; }
}
