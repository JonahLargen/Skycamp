namespace Skycamp.ApiService.Features.Weather.V1.GetForecast;

public record GetForecastRequest
{
    public required string City { get; init; }
    public int? Days { get; init; }
}