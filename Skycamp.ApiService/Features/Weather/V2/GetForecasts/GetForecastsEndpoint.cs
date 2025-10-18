using Skycamp.ApiService.Features.Weather.Shared.GetForecasts;
using Skycamp.ApiService.Common.Endpoints;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecasts;

public class GetForecastsEndpoint : EndpointWithCommandMapping<GetForecastsRequest, GetForecastsResponse, GetForecastsCommand, List<GetForecastsResult>>
{
    public GetForecastsEndpoint()
    {
        
    }

    public override void Configure()
    {
        Get("/weather/forecasts");
        Version(2);

        //AllowAnonymous();

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

    public override GetForecastsResponse MapFromEntity(List<GetForecastsResult> e)
    {
        return new GetForecastsResponse
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