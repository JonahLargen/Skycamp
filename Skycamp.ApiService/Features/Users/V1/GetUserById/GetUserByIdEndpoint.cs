using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Skycamp.ApiService.Common.Endpoints;
using Skycamp.ApiService.Features.Users.GetUserById.Shared;

namespace Skycamp.ApiService.Features.Users.V1.GetUserById;

public class GetUserByIdEndpoint : EndpointWithCommandMapping<GetUserByIdRequest, GetUserByIdResponse, GetUserByIdCommand, GetUserByIdResult>
{
    public override void Configure()
    {
        Get("/users/{UserId}");
        Version(1);

        // Require Admin role for this endpoint
        Roles("Admin");

        Description(b =>
        {
            b.WithName("GetUserByIdV1");
        });

        Summary(s =>
        {
            s.Summary = "Get user by ID";
            s.Description = "Retrieves user information by user ID. Requires Admin role.";
        });
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override GetUserByIdResponse MapFromEntity(GetUserByIdResult e)
    {
        return new GetUserByIdResponse
        {
            UserId = e.UserId,
            UserName = e.UserName,
            Email = e.Email,
            DisplayName = e.DisplayName,
            AvatarUrl = e.AvatarUrl
        };
    }

    public override GetUserByIdCommand MapToCommand(GetUserByIdRequest r)
    {
        return new GetUserByIdCommand
        {
            UserId = r.UserId
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
    public required string UserId { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}
