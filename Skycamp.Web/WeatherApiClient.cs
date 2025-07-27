namespace Skycamp.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<GetForecastResponse?> GetForecastAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<GetForecastResponse>("/weather/forecasts/v1?city=louisville&days=7", cancellationToken);
    }
}

public record GetForecastResponse
{
    public required string City { get; init; }
    public List<ForecastDay> Forecast { get; init; } = [];
}

public record ForecastDay
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}