namespace Skycamp.ApiService.Features.Weather.V1.GetForecasts;

public record GetForecastsResponse
{
    public List<GetForecastsResponseForecast> Forecasts { get; init; } = [];
}

public record GetForecastsResponseForecast
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
}