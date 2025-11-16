using FastEndpoints;
using FluentValidation;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Features.Users.GetUserById.Shared;

namespace Skycamp.ApiService.Features.Users.GetUserById.V1;

public class GetUserByIdEndpoint : EndpointWithCommandMapping<GetUserByIdRequest, GetUserByIdResponse, GetUserByIdCommand, GetUserByIdResult>
{
    public override void Configure()
    {
        Get("/users/{UserId}");
        Version(1);

        Description(b =>
        {
            b.WithName("GetUserByIdV1");
        });

        Summary(s =>
        {
            s.Summary = "Gets a user by user ID.";
            s.Description = "Retrieves the details of a specific user using their user ID.";
        });
    }

    public override async Task HandleAsync(GetUserByIdRequest r, CancellationToken ct)
    {
        await SendMappedAsync(r, ct: ct);
    }

    public override GetUserByIdCommand MapToCommand(GetUserByIdRequest r)
    {
        return new GetUserByIdCommand()
        {
            UserId = r.UserId
        };
    }

    public override GetUserByIdResponse MapFromEntity(GetUserByIdResult e)
    {
        return new GetUserByIdResponse()
        {
            Id = e.Id,
            UserName = e.UserName,
            DisplayName = e.DisplayName,
            Email = e.Email,
            AvatarUrl = e.AvatarUrl,
            CreatedUtc = e.CreatedUtc
        };
    }
}

public record GetUserByIdRequest
{
    public string UserId { get; init; } = null!;
}

public class GetUserByIdRequestValidator : Validator<GetUserByIdRequest>
{
    public GetUserByIdRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public record GetUserByIdResponse
{
    public required string Id { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? Email { get; init; }
    public string? AvatarUrl { get; init; }
    public DateTime CreatedUtc { get; init; }
}
