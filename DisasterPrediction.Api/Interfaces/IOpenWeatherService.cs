using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Interfaces;

public interface IOpenWeatherService
{
    Task<WeatherData> GetWeatherDataAsync(string cityName, string countryCode = "TH");
}