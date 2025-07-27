namespace Skycamp.ApiService.Features.Weather.Shared;

public class WeatherForecastDay
{
    public required DateOnly Date { get; set; }
    public required int TemperatureC { get; set; }
    public required string Summary { get; set; }
}