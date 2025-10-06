using System.Text.Json.Serialization;

namespace Skycamp.Web;

public class ApplicationApiClient(HttpClient httpClient)
{
    public async Task<GetForecastResponse?> GetForecastAsync(string city, int days = 7, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<GetForecastResponse>($"/weather/forecasts/v2?city={city}&days={days}", cancellationToken);
    }

    public async Task<SyncUserResponse> SyncUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/users/sync/v1", new { UserId = userId }, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SyncUserResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Failed to deserialize SyncUserResponse");
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

public record SyncUserResponse
{
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }
}