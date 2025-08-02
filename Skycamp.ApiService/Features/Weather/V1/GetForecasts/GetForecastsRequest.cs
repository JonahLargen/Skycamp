namespace Skycamp.ApiService.Features.Weather.V1.GetForecasts;

public record GetForecastsRequest
{
    public required string City { get; init; }
    public int? Days { get; init; }
}