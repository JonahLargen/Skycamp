namespace Skycamp.ApiService.Features.Weather.V2.GetForecast;

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
    public required string Description { get; init; }
}