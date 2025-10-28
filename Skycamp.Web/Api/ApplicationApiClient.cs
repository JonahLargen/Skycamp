using System.Text.Json.Serialization;

namespace Skycamp.Web.Api;

public class ApplicationApiClient(HttpClient httpClient) : BaseApiClient
{
    public async Task<ApiDataResult<GetForecastResponse>> GetForecastAsync(string city, int days = 7, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/weather/forecasts/v2?city={city}&days={days}", cancellationToken);

        return await CreateApiDataResultAsync<GetForecastResponse>(response);
    }

    public async Task<ApiDataResult<CreateWorkspaceResponse>> CreateWorkspaceAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/projectmanagement/workspaces/v1", request, cancellationToken);

        return await CreateApiDataResultAsync<CreateWorkspaceResponse>(response);
    }

    public async Task<ApiResult> EditWorkspaceAsync(Guid workspaceId, EditWorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/projectmanagement/workspaces/{workspaceId}/v1", request, cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/projectmanagement/workspaces/{workspaceId}/v1", cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiDataResult<CreateProjectResponse>> CreateProjectAsync(Guid workspaceId, CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/projectmanagement/workspaces/{workspaceId}/projects/v1", request, cancellationToken);

        return await CreateApiDataResultAsync<CreateProjectResponse>(response);
    }

    public async Task<ApiResult> EditProjectAsync(Guid workspaceId, Guid projectId, EditProjectRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/projectmanagement/workspaces/{workspaceId}/projects/{projectId}/v1", request, cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteProjectAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/projectmanagement/workspaces/{workspaceId}/projects/{projectId}/v1", cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiDataResult<GetWorkspacesResponse>> GetWorkspacesAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("/projectmanagement/workspaces/v1", cancellationToken);

        return await CreateApiDataResultAsync<GetWorkspacesResponse>(response);
    }

    public async Task<ApiDataResult<GetProjectsByWorkspaceIdResponse>> GetProjectsByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/projectmanagement/workspaces/{workspaceId}/projects/v1", cancellationToken);

        return await CreateApiDataResultAsync<GetProjectsByWorkspaceIdResponse>(response);
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

public record EditWorkspaceRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public record CreateProjectRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool IsAllAccess { get; init; }
}

public record EditProjectRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool IsAllAccess { get; init; }
}

public record CreateProjectResponse
{
    public required string Id { get; init; }
}

public record GetWorkspacesResponse
{
    public List<GetWorkspacesResponseItem> Workspaces { get; init; } = [];
}

public record GetWorkspacesResponseItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string RoleName { get; init; } = null!;
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}

public record GetProjectsByWorkspaceIdResponse
{
    public List<GetProjectsByWorkspaceIdResponseItem> Projects { get; init; } = [];
}

public record GetProjectsByWorkspaceIdResponseItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string RoleName { get; init; }
    public required bool IsAllAccess { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
}