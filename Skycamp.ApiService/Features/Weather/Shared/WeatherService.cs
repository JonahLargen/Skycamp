namespace Skycamp.ApiService.Features.Weather.Shared;

public class WeatherService : IWeatherService
{
    public Task<List<WeatherForecastDay>> GetForecastsAsync(string city, int days)
    {
        // Simulate data
        var result = new List<WeatherForecastDay>();

        for (int i = 0; i < days; i++)
        {
            result.Add(new WeatherForecastDay
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                TemperatureC = 20 + i,
                Summary = "Sunny",
                Description = "A sunny day with clear skies."
            });
        }

        return Task.FromResult(result);
    }
}