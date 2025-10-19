using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Skycamp.Web;

public class ApplicationApiClient(HttpClient httpClient)
{
    public async Task<GetForecastResponse?> GetForecastAsync(string city, int days = 7, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<GetForecastResponse>($"/weather/forecasts/v2?city={city}&days={days}", cancellationToken);
    }

    public async Task<SyncUserResponse> SyncUserAsync(SyncUserRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/users/sync/v1", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SyncUserResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Failed to deserialize SyncUserResponse");
    }

    public async Task<CreateWorkspaceResponse> CreateWorkspaceAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/projectmanagement/workspaces/v1", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreateWorkspaceResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Failed to deserialize CreateWorkspaceResponse");
    }

    public async Task<GetWorkspacesResponse> GetWorkspacesAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/projectmanagement/workspaces/v1", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetWorkspacesResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Failed to deserialize GetWorkspacesResponse");
    }
}

public class AccessTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

public record GetForecastResponse
{
    [JsonPropertyName("forecasts")]
    public List<ForecastDay> Forecasts { get; init; } = [];
}

public record ForecastDay
{
    [JsonPropertyName("date")]
    public required DateOnly Date { get; init; }

    [JsonPropertyName("temperatureC")]
    public required int TemperatureC { get; init; }

    [JsonPropertyName("summary")]
    public required string Summary { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record SyncUserRequest
{
    [JsonPropertyName("loginProvider")]
    public required string LoginProvider { get; init; }

    [JsonPropertyName("providerKey")]
    public required string ProviderKey { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; init; }
}

public record SyncUserResponse
{
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    [JsonPropertyName("created")]
    public required bool Created { get; init; }

    [JsonPropertyName("roles")]
    public required List<string> Roles { get; init; }
}

public record CreateWorkspaceRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public record CreateWorkspaceResponse
{
    public required string Id { get; init; }
}

public record GetWorkspacesResponse
{
    public required List<GetWorkspacesResponseItem> Workspaces { get; init; }
}

public record GetWorkspacesResponseItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}