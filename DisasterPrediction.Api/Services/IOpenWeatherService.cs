using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public interface IOpenWeatherService
{
    Task<WeatherData> GetWeatherDataAsync(decimal latitude, decimal longitude);
}