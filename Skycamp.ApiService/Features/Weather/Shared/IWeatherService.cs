namespace Skycamp.ApiService.Features.Weather.Shared;

public interface IWeatherService
{
    Task<List<WeatherForecastDay>> GetForecastAsync(string city, int days);
}