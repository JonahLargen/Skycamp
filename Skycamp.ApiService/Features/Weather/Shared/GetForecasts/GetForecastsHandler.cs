using FastEndpoints;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public class GetForecastsHandler : CommandHandler<GetForecastsCommand, List<Forecast>>
{
    public override Task<List<Forecast>> ExecuteAsync(GetForecastsCommand command, CancellationToken ct)
    {
        // Simulate data
        var result = new List<Forecast>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < command.Days; i++)
        {
            result.Add(new Forecast
            {
                Date = DateOnly.FromDateTime(now.AddDays(i)),
                TemperatureC = 20 + i,
                Summary = "Sunny",
                Description = "A sunny day with clear skies."
            });
        }

        //AddError("an error added by the endpoint!");

        //ThrowIfAnyErrors();

        return Task.FromResult(result);
    }
}