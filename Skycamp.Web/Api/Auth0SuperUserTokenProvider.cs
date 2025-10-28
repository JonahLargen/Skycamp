using System.Text.Json;

namespace Skycamp.Web.Api;

public class Auth0SuperUserTokenProvider : ITokenProvider
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private string? accessToken;
    private DateTime tokenExpiresAt = DateTime.MinValue;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public Auth0SuperUserTokenProvider(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(accessToken) && tokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
            return accessToken;

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            if (!string.IsNullOrEmpty(accessToken) && tokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
                return accessToken;

            var auth0Domain = configuration["Auth0:Domain"];
            var auth0ClientId = configuration["Auth0:ClientId"];
            var auth0ClientSecret = configuration["Auth0:ClientSecret"];
            var auth0Audience = configuration["Auth0:Audience"];
            var auth0SuperUserEmail = configuration["Auth0:SuperUserEmail"];
            var auth0SuperUserPassword = configuration["Auth0:SuperUserPassword"];

            if (string.IsNullOrEmpty(auth0Domain) || string.IsNullOrEmpty(auth0ClientId) || string.IsNullOrEmpty(auth0ClientSecret) ||
                string.IsNullOrEmpty(auth0Audience) || string.IsNullOrEmpty(auth0SuperUserEmail) || string.IsNullOrEmpty(auth0SuperUserPassword))
            {
                throw new InvalidOperationException("Auth0 configuration is missing. Please ensure all required keys are set.");
            }

            var dict = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", auth0SuperUserEmail },
                { "password", auth0SuperUserPassword },
                { "audience", auth0Audience },
                { "scope", "openid" },
                { "client_id", auth0ClientId },
                { "client_secret", auth0ClientSecret }
            };

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"https://{auth0Domain}/oauth/token")
            {
                Content = new FormUrlEncodedContent(dict)
            };
            var tokenResponse = await httpClient.SendAsync(tokenRequest, cancellationToken);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenResult = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

            string? newAccessToken = null;
            int expiresIn = 3600;
            using var doc = JsonDocument.Parse(tokenResult);

            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
                newAccessToken = accessTokenElement.GetString();
            if (doc.RootElement.TryGetProperty("expires_in", out var expiresInElement))
                expiresIn = expiresInElement.GetInt32();

            if (string.IsNullOrEmpty(newAccessToken))
                throw new InvalidOperationException("Failed to get access token from Auth0.");

            accessToken = newAccessToken;
            tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

            return accessToken;
        }
        finally
        {
            semaphore.Release();
        }
    }
}