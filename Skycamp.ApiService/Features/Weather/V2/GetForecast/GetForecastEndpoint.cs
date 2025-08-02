using FastEndpoints;
using Skycamp.ApiService.Features.Weather.Shared;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecast;

public class GetForecastEndpoint : EndpointWithMapping<GetForecastRequest, GetForecastResponse, List<WeatherForecastDay>>
{
    private readonly IWeatherService _weatherService;

    public GetForecastEndpoint(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public override void Configure()
    {
        Get("/weather/forecasts");
        Version(2);

        AllowAnonymous();

        //Description("Gets weather forecast for a city.");
    }

    public override async Task HandleAsync(GetForecastRequest req, CancellationToken ct)
    {
        var forecasts = await _weatherService.GetForecastsAsync(req.City, req.Days ?? 1);
        var response = MapFromEntity(forecasts);

        await Send.OkAsync(response, ct);
    }

    public override GetForecastResponse MapFromEntity(List<WeatherForecastDay> e)
    {
        return new GetForecastResponse
        {
            Forecasts = e.Select(day => new Forecast
            {
                Date = day.Date,
                TemperatureC = day.TemperatureC,
                Summary = day.Summary,
                Description = day.Description
            }).ToList()
        };
    }
}