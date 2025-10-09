using FastEndpoints;

namespace Skycamp.ApiService.Features.Weather.Shared.GetForecasts;

public class GetForecastsCommandHandler : CommandHandler<GetForecastsCommand, List<GetForecastsResult>>
{
    public override Task<List<GetForecastsResult>> ExecuteAsync(GetForecastsCommand command, CancellationToken ct)
    {
        // Simulate data
        var result = new List<GetForecastsResult>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < command.Days; i++)
        {
            result.Add(new GetForecastsResult
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