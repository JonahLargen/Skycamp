using FastEndpoints;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public class GetForecastsCommand : ICommand<List<Forecast>>
{
    public string City { get; set; } = string.Empty;
    public int? Days { get; set; } = 1;
}