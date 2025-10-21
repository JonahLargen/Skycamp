using System.Security.Claims;

namespace Skycamp.ApiService.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetRequiredUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("sub") ?? throw new InvalidOperationException("User is not authenticated or does not have a sub/username claim");
    }

    public static string? GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("sub");
    }
}