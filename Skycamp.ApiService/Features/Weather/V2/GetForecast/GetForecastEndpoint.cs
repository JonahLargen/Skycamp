using FastEndpoints;
using Skycamp.ApiService.Features.Weather.Shared;

namespace Skycamp.ApiService.Features.Weather.V2.GetForecast;

public class GetForecastEndpoint : Endpoint<GetForecastRequest, GetForecastResponse>
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
        var forecast = await _weatherService.GetForecastAsync(req.City, req.Days ?? 1);

        Response = new GetForecastResponse
        {
            City = req.City,
            Forecast = forecast.Select(day => new ForecastDay
            {
                Date = day.Date,
                TemperatureC = day.TemperatureC,
                Summary = day.Summary,
                Description = day.Description
            }).ToList()
        };
    }
}