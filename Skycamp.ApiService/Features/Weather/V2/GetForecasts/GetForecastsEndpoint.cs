using Skycamp.ApiService.Features.Weather.Shared;
using Skycamp.ApiService.Features.Weather.Shared.GetForecasts;
using Skycamp.ApiService.Shared.FastEndpoints;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public class GetForecastsEndpoint : EndpointWithCommandMapping<GetForecastsRequest, GetForecastResponse, GetForecastsCommand, List<Forecast>>
{
    public GetForecastsEndpoint()
    {
        
    }

    public override void Configure()
    {
        Get("/weather/forecasts");
        Version(2);

        AllowAnonymous();

        Description(b =>
        {
            b.WithName("GetForecastV2");
        });

        Summary(s =>
        {
            s.Summary = "Get weather forecast for a city";
            s.Description = "Retrieves the weather forecast for a specified city for a given number of days.";
        });
    }

    public override async Task HandleAsync(GetForecastsRequest req, CancellationToken ct)
    {
        await SendMappedAsync(req, ct: ct);
    }

    public override GetForecastsCommand MapToCommand(GetForecastsRequest r)
    {
        return new GetForecastsCommand()
        {
            City = r.City,
            Days = r.Days ?? 1
        };
    }

    public override GetForecastResponse MapFromEntity(List<Forecast> e)
    {
        return new GetForecastResponse
        {
            Forecasts = e.Select(day => new GetForecastResponseForecast
            {
                Date = day.Date,
                TemperatureC = day.TemperatureC,
                Summary = day.Summary,
                Description = day.Description
            }).ToList()
        };
    }
}