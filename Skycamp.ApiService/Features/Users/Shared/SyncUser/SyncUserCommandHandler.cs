using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

public class SyncUserCommandHandler : CommandHandler<SyncUserCommand, SyncUserCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SyncUserCommandHandler> _logger;

    public SyncUserCommandHandler(UserManager<ApplicationUser> userManager, ILogger<SyncUserCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override async Task<SyncUserCommandResponse> ExecuteAsync(SyncUserCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);

        if (user == null)
        {
            user = new ApplicationUser
            {
                Id = command.UserId,
                UserName = command.UserId
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                _logger.LogError("Failed to create user with ID {UserId}. Errors: {@Errors}", command.UserId, createResult.Errors);

                ThrowError("Failed to create user");
            }
        }
        else
        {
            //update if needed
        }

        return new SyncUserCommandResponse()
        {
            UserId = user.Id
        };
    }
}