namespace Skycamp.ApiService.Features.Weather.Shared;

public interface IWeatherService
{
    Task<List<WeatherForecastDay>> GetForecastsAsync(string city, int days);
}