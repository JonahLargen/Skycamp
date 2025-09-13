using System.Text.Json.Serialization;

namespace Skycamp.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<GetForecastResponse?> GetForecastAsync(string city, int days = 7, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<GetForecastResponse>($"/weather/forecasts/v2?city={city}&days={days}", cancellationToken);
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