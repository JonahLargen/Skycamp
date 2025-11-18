using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.Users.GetUserById.Shared;

public class GetUserByIdCommandHandler : CommandHandler<GetUserByIdCommand, GetUserByIdResult?>
{
    private readonly ILogger<GetUserByIdCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdCommandHandler(ILogger<GetUserByIdCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public override async Task<GetUserByIdResult?> ExecuteAsync(GetUserByIdCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == command.UserId)
            .Select(u => new GetUserByIdResult
            {
                Id = u.Id,
                UserName = u.UserName,
                DisplayName = u.DisplayName,
                Email = u.Email,
                AvatarUrl = u.AvatarUrl,
                CreatedUtc = u.CreatedUtc
            })
            .FirstOrDefaultAsync(ct);

        if (user == null)
        {
            ThrowError("User does not exist", statusCode: 404);
        }

        return user;
    }
}

public record GetUserByIdCommand : ICommand<GetUserByIdResult>
{
    public required string UserId { get; init; }
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
    public required string Id { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? Email { get; init; }
    public string? AvatarUrl { get; init; }
    public DateTime CreatedUtc { get; init; }
}
