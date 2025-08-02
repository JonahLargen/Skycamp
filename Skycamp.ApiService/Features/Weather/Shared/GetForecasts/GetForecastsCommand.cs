using FastEndpoints;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public class GetForecastsCommand : ICommand<List<Forecast>>
{
    public required string City { get; set; }
    public int? Days { get; set; } = 1;
}