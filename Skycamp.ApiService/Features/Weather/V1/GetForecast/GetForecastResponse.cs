namespace Skycamp.ApiService.Features.Weather.V1.GetForecast;

public record GetForecastResponse
{
    public List<Forecast> Forecasts { get; init; } = [];
}

public record Forecast
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
}