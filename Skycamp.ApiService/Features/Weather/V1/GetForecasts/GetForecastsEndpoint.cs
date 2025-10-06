using FastEndpoints;
using Skycamp.ApiService.Data;
using Skycamp.ApiService.Features.Weather.Shared;
using Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

namespace Skycamp.ApiService.Features.Weather.V1.GetForecasts;

public class GetForecastsEndpoint : EndpointWithMapping<GetForecastsRequest, GetForecastsResponse, List<Forecast>>
{
    public ApplicationDbContext Context { get; set; } = null!;

    public GetForecastsEndpoint()
    {
        
    }

    public override void Configure()
    {
        Get("/weather/forecasts");
        Version(1);

        AllowAnonymous();

        Description(b =>
        {
            b.WithName("GetForecastV1");
        });

        Summary(s =>
        {
            s.Summary = "Get weather forecast for a city";
            s.Description = "Retrieves the weather forecast for a specified city for a given number of days.";
        });
    }

    public override async Task HandleAsync(GetForecastsRequest req, CancellationToken ct)
    {
        var command = MapToCommand(req);
        var forecasts = await command.ExecuteAsync(ct);
        var response = MapFromEntity(forecasts);

        await Send.OkAsync(response, ct);
    }

    public GetForecastsCommand MapToCommand(GetForecastsRequest req)
    {
        return new GetForecastsCommand()
        {
            City = req.City,
            Days = req.Days ?? 1
        };
    }


    public override GetForecastsResponse MapFromEntity(List<Forecast> e)
    {
        return new GetForecastsResponse
        {
            Forecasts = e.Select(day => new GetForecastsResponseForecast
            {
                Date = day.Date,
                TemperatureC = day.TemperatureC,
                Summary = day.Summary
            }).ToList()
        };
    }
}