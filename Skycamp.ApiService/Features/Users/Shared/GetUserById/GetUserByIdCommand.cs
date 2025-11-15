using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.Users.GetUserById.Shared;

public class GetUserByIdCommandHandler : CommandHandler<GetUserByIdCommand, GetUserByIdResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<GetUserByIdResult> ExecuteAsync(GetUserByIdCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);

        if (user == null)
        {
            ThrowError("User not found", statusCode: 404);
        }

        return new GetUserByIdResult
        {
            UserId = user!.Id,
            UserName = user.UserName,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl
        };
    }
}

public record GetUserByIdCommand : ICommand<GetUserByIdResult>
{
    public required string UserId { get; set; }
}

public class GetUserByIdCommandValidator : AbstractValidator<GetUserByIdCommand>
{
    public GetUserByIdCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public record GetUserByIdResult
{
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
}
