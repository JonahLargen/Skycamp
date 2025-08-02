namespace Skycamp.ApiService.Features.Weather.Shared;

public record WeatherForecastDay
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
    public required string Description { get; init; }
}