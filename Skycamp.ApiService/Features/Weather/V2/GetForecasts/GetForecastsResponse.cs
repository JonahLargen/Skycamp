namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public record GetForecastResponse
{
    public List<GetForecastResponseForecast> Forecasts { get; init; } = [];
}

public record GetForecastResponseForecast
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
    public required string Description { get; init; }
}