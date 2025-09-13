using Skycamp.ApiService.Common.Logging;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

[Loggable]
public record GetForecastsRequest
{
    public required string City { get; init; }

    [LogIf(nameof(CityIsLouisville))]
    public int? Days { get; init; }

    public bool CityIsLouisville => string.Equals(City, "Louisville", StringComparison.OrdinalIgnoreCase);
}