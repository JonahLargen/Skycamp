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

    public async Task<ApiResult> EditProjectDatesAsync(Guid workspaceId, Guid projectId, EditProjectDatesRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/projectmanagement/workspaces/{workspaceId}/projects/{projectId}/updatedates/v1", request, cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiResult> EditProjectProgressAsync(Guid workspaceId, Guid projectId, EditProjectProgressRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/projectmanagement/workspaces/{workspaceId}/projects/{projectId}/updateprogress/v1", request, cancellationToken);

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

    public async Task<ApiDataResult<GetProjectByIdResponse>> GetProjectByIdAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/projectmanagement/workspaces/{workspaceId}/projects/{projectId}/v1", cancellationToken);

        return await CreateApiDataResultAsync<GetProjectByIdResponse>(response);
    }

    public async Task<ApiDataResult<GetTodosByProjectResponse>> GetTodosByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/todomanagement/projects/{projectId}/todos/v1", cancellationToken);

        return await CreateApiDataResultAsync<GetTodosByProjectResponse>(response);
    }

    public async Task<ApiDataResult<CreateTodoResponse>> CreateTodoAsync(Guid projectId, CreateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/todomanagement/projects/{projectId}/todos/v1", request, cancellationToken);

        return await CreateApiDataResultAsync<CreateTodoResponse>(response);
    }

    public async Task<ApiResult> UpdateTodoAsync(Guid todoId, UpdateTodoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/todomanagement/todos/{todoId}/v1", request, cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteTodoAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/todomanagement/todos/{todoId}/v1", cancellationToken);

        return await CreateApiResultAsync(response);
    }

    public async Task<ApiDataResult<ToggleTodoCompleteResponse>> ToggleTodoCompleteAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/todomanagement/todos/{todoId}/toggle-complete/v1", null, cancellationToken);

        return await CreateApiDataResultAsync<ToggleTodoCompleteResponse>(response);
    }

    public async Task<ApiDataResult<GetProjectActivitiesResponse>> GetProjectActivitiesAsync(Guid projectId, int? limit = null, CancellationToken cancellationToken = default)
    {
        var url = $"/projectmanagement/projects/{projectId}/activities/v1";
        if (limit.HasValue)
        {
            url += $"?limit={limit.Value}";
        }

        var response = await httpClient.GetAsync(url, cancellationToken);

        return await CreateApiDataResultAsync<GetProjectActivitiesResponse>(response);
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

public record EditProjectDatesRequest
{
    public required DateOnly? StartDate { get; init; }
    public required DateOnly? EndDate { get; init; }
}

public record EditProjectProgressRequest
{
    public required decimal Progress { get; init; }
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
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string RoleName { get; init; } = null!;
    public bool IsAllAccess { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public record GetProjectByIdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string RoleName { get; init; } = null!;
    public bool IsAllAccess { get; init; }
    public string? CreateUserId { get; init; }
    public string? CreateUserDisplayName { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime LastUpdatedUtc { get; init; }
    public decimal Progress { get; set; }
    public DateTime? ArchivedUtc { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<GetProjectByIdResponseUser> Users { get; init; } = [];
}

public record GetProjectByIdResponseUser
{
    public string Id { get; init; } = null!;
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string RoleName { get; init; } = null!;
    public string? AvatarUrl { get; init; }
}

public record GetTodosByProjectResponse
{
    public List<GetTodosByProjectResponseItem> Todos { get; init; } = [];
}

public record GetTodosByProjectResponseItem
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public DateOnly? DueDate { get; set; }
    public string? PrimaryAssigneeId { get; set; }
    public string? PrimaryAssigneeDisplayName { get; set; }
    public string? PrimaryAssigneeAvatarUrl { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public string? CreateUserId { get; set; }
    public string? CreateUserDisplayName { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}

public record CreateTodoRequest
{
    public required string Text { get; init; }
    public DateOnly? DueDate { get; init; }
    public string? PrimaryAssigneeId { get; init; }
    public string? Notes { get; init; }
}

public record CreateTodoResponse
{
    public required Guid Id { get; init; }
}

public record UpdateTodoRequest
{
    public required string Text { get; init; }
    public DateOnly? DueDate { get; init; }
    public string? PrimaryAssigneeId { get; init; }
    public string? Notes { get; init; }
}

public record ToggleTodoCompleteResponse
{
    public required bool IsCompleted { get; init; }
}

public record GetProjectActivitiesResponse
{
    public List<GetProjectActivitiesResponseItem> Activities { get; init; } = [];
}

public record GetProjectActivitiesResponseItem
{
    public required string UserName { get; init; }
    public string? UserAvatar { get; init; }
    public required string Description { get; init; }
    public DateTime Timestamp { get; init; }
}
