using System.Net.Http.Headers;

namespace Skycamp.Web.Api;

public class AdminApiClient : BaseApiClient
{
    private readonly HttpClient httpClient;
    private readonly ITokenProvider tokenProvider;

    public AdminApiClient(HttpClient httpClient, [FromKeyedServices("Auth0SuperUser")] ITokenProvider tokenProvider) 
    {
        this.httpClient = httpClient;
        this.tokenProvider = tokenProvider;
    }

    public async Task<ApiDataResult<SyncUserResponse>> SyncUserAsync(SyncUserRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = await BuildHttpRequest(HttpMethod.Post, "/users/sync/v1", request, cancellationToken);
        var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        return await CreateApiDataResultAsync<SyncUserResponse>(response);
    }

    public async Task<ApiDataResult<GetUserByIdResponse>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var httpRequest = await BuildHttpRequest(HttpMethod.Get, $"/users/{userId}/v1", null, cancellationToken);
        var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        return await CreateApiDataResultAsync<GetUserByIdResponse>(response);
    }

    private async Task<HttpRequestMessage> BuildHttpRequest(HttpMethod method, string url, object? content = null, CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken);

        var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }

        return request;
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