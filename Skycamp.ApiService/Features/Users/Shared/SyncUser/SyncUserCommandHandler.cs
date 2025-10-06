using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Data.Identity;

namespace Skycamp.ApiService.Features.Users.Shared.SyncUser;

public class SyncUserCommandHandler : CommandHandler<SyncUserCommand, SyncUserCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SyncUserCommandHandler> _logger;

    public SyncUserCommandHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, ILogger<SyncUserCommandHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<SyncUserCommandResponse> ExecuteAsync(SyncUserCommand command, CancellationToken ct = default)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        var created = false;
        ApplicationUser? user;

        try
        {
            user = await _userManager.FindByLoginAsync(command.LoginProvider, command.ProviderKey);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserName = command.ProviderKey,
                    Email = command.Email,
                    EmailConfirmed = command.EmailVerified,
                    DisplayName = command.DisplayName,
                    AvatarUrl = command.AvatarUrl,
                    CreatedUtc = DateTime.UtcNow,
                    LastLoginUtc = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    ThrowError("Failed to create user");
                }

                var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(command.LoginProvider, command.ProviderKey, command.LoginProvider));

                if (!addLoginResult.Succeeded)
                {
                    ThrowError("Failed to add user login");
                }

                created = true;
            }
            else
            {
                user.Email = command.Email;
                user.EmailConfirmed = command.EmailVerified;
                user.LastLoginUtc = DateTime.UtcNow;
                user.DisplayName = command.DisplayName;
                user.AvatarUrl = command.AvatarUrl;

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    ThrowError("Failed to update user");
                }
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new SyncUserCommandResponse()
        {
            UserId = user.Id,
            Created = created,
            Roles = roles.ToList()
        };
    }
}