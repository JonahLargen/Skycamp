using FastEndpoints;
using Skycamp.ApiService.Common.Logging;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

[Loggable]
public class GetForecastsCommand : ICommand<List<Forecast>>
{
    public required string City { get; set; }
    public int? Days { get; set; } = 1;
}