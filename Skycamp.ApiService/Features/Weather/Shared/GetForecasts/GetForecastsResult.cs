namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public record GetForecastsResult
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
    public required string Description { get; init; }
}