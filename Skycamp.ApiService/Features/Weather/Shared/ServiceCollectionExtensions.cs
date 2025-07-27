namespace Skycamp.ApiService.Features.Weather.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherServices(this IServiceCollection services)
    {
        services.AddScoped<IWeatherService, WeatherService>();

        return services;
    }
}