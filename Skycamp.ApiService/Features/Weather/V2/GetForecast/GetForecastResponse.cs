namespace Skycamp.ApiService.Features.Weather.V2.GetForecast;

public record GetForecastResponse
{
    public List<Forecast> Forecasts { get; init; } = [];
}

public record Forecast
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
    public required string Description { get; init; }
}