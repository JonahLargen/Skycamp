namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public record GetForecastsRequest
{
    public required string City { get; init; }
    public int? Days { get; init; }
}